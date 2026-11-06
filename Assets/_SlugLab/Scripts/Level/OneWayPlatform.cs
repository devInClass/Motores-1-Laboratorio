using SlugLab.Player;
using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Plataforma atravesable desde abajo (one-way). Con ↓ + salto el jugador
    /// la atraviesa hacia abajo durante un instante.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class OneWayPlatform : MonoBehaviour
    {
        [SerializeField] private float dropThroughTime = 0.35f;

        private Collider2D _platformCollider;

        private void Awake() => _platformCollider = GetComponent<Collider2D>();

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag("Player")) return;

            PlayerInput input = collision.gameObject.GetComponent<PlayerInput>();
            if (input == null || !input.CrouchHeld || !input.JumpPressed) return;

            Collider2D playerCollider = collision.collider;
            if (playerCollider == null) return;

            Physics2D.IgnoreCollision(playerCollider, _platformCollider, true);
            StartCoroutine(RestoreCollision(playerCollider));
        }

        private System.Collections.IEnumerator RestoreCollision(Collider2D playerCollider)
        {
            yield return new WaitForSeconds(dropThroughTime);
            if (playerCollider != null) Physics2D.IgnoreCollision(playerCollider, _platformCollider, false);
        }
    }
}
