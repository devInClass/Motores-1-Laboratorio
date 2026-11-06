using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Zona letal (el fondo de un precipicio). Aplica daño masivo al jugador.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class KillZone : MonoBehaviour
    {
        [SerializeField] private int damage = 999;

        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            Combat.IDamageable target = other.GetComponentInParent<Combat.IDamageable>();
            if (target != null)
            {
                target.ApplyDamage(new Combat.DamageInfo(damage, other.transform.position, Vector2.up, 0f, gameObject));
            }
        }
    }
}
