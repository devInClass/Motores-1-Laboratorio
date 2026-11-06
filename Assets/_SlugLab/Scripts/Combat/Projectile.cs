using UnityEngine;

namespace SlugLab.Combat
{
    /// <summary>
    /// Bala o proyectil genérico. Se mueve con el Rigidbody (cinemático) para que
    /// las colisiones las resuelva la física y no nosotros a mano.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Vuelo")]
        [SerializeField] private int damage = 1;
        [SerializeField] private float speed = 18f;
        [SerializeField] private float lifetime = 2.5f;
        [SerializeField] private float knockback = 4f;

        [Header("Colisión")]
        [Tooltip("Capas contra las que choca la bala (suelo, enemigos, props).")]
        [SerializeField] private LayerMask hitMask;

        [Header("Efectos")]
        [SerializeField] private GameObject impactVfx;
        [SerializeField] private AudioClip impactSfx;

        private Rigidbody2D _rb;
        private Collider2D _collider;
        private Team _team;
        private float _despawnAt;
        private bool _flying;
        private ProjectilePool _pool;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 0f;
        }

        public void SetPool(ProjectilePool pool) => _pool = pool;

        public void Launch(Vector2 direction, Team team, int overrideDamage = -1, float overrideSpeed = -1f)
        {
            _team = team;
            if (overrideDamage >= 0) damage = overrideDamage;
            if (overrideSpeed > 0f) speed = overrideSpeed;

            Vector2 dir = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
            _rb.linearVelocity = dir * speed;
            transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

            _despawnAt = Time.time + lifetime;
            _flying = true;
        }

        private void Update()
        {
            if (_flying && Time.time >= _despawnAt) Despawn(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_flying || other == null) return;
            if ((hitMask.value & (1 << other.gameObject.layer)) == 0) return;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target != null && target.Team != _team)
            {
                Vector2 direction = _rb.linearVelocity.normalized;
                Vector2 point = other.ClosestPoint(transform.position);
                target.ApplyDamage(new DamageInfo(damage, point, direction, knockback, gameObject));
            }

            Despawn(true);
        }

        private void Despawn(bool showImpact)
        {
            if (!_flying) return;
            _flying = false;
            _rb.linearVelocity = Vector2.zero;

            if (showImpact)
            {
                if (impactVfx != null) Instantiate(impactVfx, transform.position, Quaternion.identity);
                if (impactSfx != null) Audio.AudioManager.PlaySfx(impactSfx, 0.5f);
            }

            if (_pool != null) _pool.Release(this);
            else gameObject.SetActive(false);
        }
    }
}
