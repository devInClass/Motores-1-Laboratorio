using SlugLab.Combat;
using SlugLab.Utils;
using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Pinchos, lava o cualquier zona que hace daño mientras el jugador está
    /// dentro. Usa un cooldown para no quitar toda la vida en un frame.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Hazard : MonoBehaviour
    {
        [SerializeField] private int damage = 1;
        [SerializeField] private float damageInterval = 0.9f;
        [SerializeField] private float knockback = 8f;

        private readonly Cooldown _cooldown = new Cooldown(0.9f);

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            _cooldown.Duration = damageInterval;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (!other.CompareTag("Player") || !_cooldown.TryConsume(Time.time)) return;

            IDamageable target = other.GetComponentInParent<IDamageable>();
            if (target == null) return;

            Vector2 direction = new Vector2(other.transform.position.x - transform.position.x, 0.6f).normalized;
            target.ApplyDamage(new DamageInfo(damage, other.ClosestPoint(transform.position), direction, knockback, gameObject));
        }
    }
}
