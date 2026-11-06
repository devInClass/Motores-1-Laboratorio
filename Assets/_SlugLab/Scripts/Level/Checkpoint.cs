using SlugLab.Core;
using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Punto de control: al tocarlo se guarda la posición de reaparición y se
    /// cura al jugador. La bandera cambia de color para dar feedback.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private Color inactiveColor = new Color(0.45f, 0.45f, 0.5f);
        [SerializeField] private Color activeColor = new Color(0.35f, 1f, 0.55f);
        [SerializeField] private SpriteRenderer flagRenderer;
        [SerializeField] private Transform respawnPoint;

        private bool _activated;

        private void Awake()
        {
            GetComponent<Collider2D>().isTrigger = true;
            if (flagRenderer != null) flagRenderer.color = inactiveColor;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_activated || !other.CompareTag("Player")) return;
            _activated = true;

            Vector3 position = respawnPoint != null ? respawnPoint.position : transform.position + Vector3.up;
            GameSession.SetCheckpoint(position);

            Combat.Damageable damageable = other.GetComponentInParent<Combat.Damageable>();
            if (damageable != null) damageable.HealToFull();

            if (flagRenderer != null) flagRenderer.color = activeColor;
            AudioClip clip = Resources.Load<AudioClip>("SFX/checkpoint");
            if (clip != null) Audio.AudioManager.PlaySfx(clip, 0.8f);
        }
    }
}
