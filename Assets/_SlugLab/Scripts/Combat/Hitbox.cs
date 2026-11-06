using System.Collections.Generic;
using UnityEngine;

namespace SlugLab.Combat
{
    /// <summary>
    /// Zona que hace daño (golpe cuerpo a cuerpo, explosión, láser...).
    /// Es un collider en modo "Trigger": no empuja, solo detecta.
    /// </summary>
    /// <remarks>
    /// Separar HITBOX (lo que daña) de HURTBOX (el cuerpo que recibe) es la base
    /// del combate en 2D: el jugador puede tener el cuerpo en la capa Player y
    /// el cuchillo en PlayerAttack, y así nunca se daña a sí mismo.
    /// </remarks>
    [RequireComponent(typeof(Collider2D))]
    public class Hitbox : MonoBehaviour
    {
        [Header("Daño")]
        [SerializeField] private int damage = 1;
        [SerializeField] private float knockback = 5f;

        [Header("Filtro")]
        [Tooltip("Capas a las que SÍ hace daño esta hitbox.")]
        [SerializeField] private LayerMask targetMask;

        [Header("Comportamiento")]
        [SerializeField] private bool oncePerTarget = true;
        [SerializeField] private float activeTime = 0.15f;

        [Header("Efectos")]
        [SerializeField] private GameObject hitEffectPrefab;

        public int Damage { get => damage; set => damage = Mathf.Max(0, value); }
        public LayerMask TargetMask { get => targetMask; set => targetMask = value; }

        private Collider2D _collider;
        private readonly HashSet<int> _alreadyHit = new();
        private float _disableAt = -1f;

        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _collider.isTrigger = true;
            _collider.enabled = false;
        }

        private void OnEnable()
        {
            _alreadyHit.Clear();
        }

        /// <summary>Enciende la hitbox durante <paramref name="duration"/> segundos.</summary>
        public void Activate(float duration)
        {
            _alreadyHit.Clear();
            _collider.enabled = true;
            _disableAt = duration > 0f ? Time.time + duration : -1f;

            // Si ya había algo dentro al encenderse, lo detectamos en este frame.
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(targetMask);
            filter.useTriggers = true;

            Collider2D[] results = new Collider2D[8];
            int count = Physics2D.OverlapCollider(_collider, filter, results);
            for (int i = 0; i < count; i++) TryHit(results[i]);
        }

        public void Deactivate()
        {
            _disableAt = -1f;
            if (_collider != null) _collider.enabled = false;
        }

        private void Update()
        {
            if (_disableAt > 0f && Time.time >= _disableAt) Deactivate();
        }

        private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
        private void OnTriggerStay2D(Collider2D other) => TryHit(other);

        private bool TryHit(Collider2D other)
        {
            if (other == null || !_collider.enabled) return false;
            if ((targetMask.value & (1 << other.gameObject.layer)) == 0) return false;

            int id = other.GetInstanceID();
            if (oncePerTarget && _alreadyHit.Contains(id)) return false;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null || !target.IsAlive || target.Team == Team.Neutral) return false;

            Vector2 point = other.ClosestPoint(transform.position);
            Vector2 direction = (other.transform.position - transform.position).normalized;
            DamageInfo info = new DamageInfo(damage, point, direction, knockback, gameObject);

            if (!target.ApplyDamage(info)) return false;

            if (oncePerTarget) _alreadyHit.Add(id);
            if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, point, Quaternion.identity);
            return true;
        }
    }
}
