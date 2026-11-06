using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Puerta o barrera que se abre cuando el jugador limpia una zona.
    /// </summary>
    public class LevelGate : MonoBehaviour
    {
        [SerializeField] private GameObject barrier;
        [SerializeField] private SpriteRenderer indicator;
        [SerializeField] private Color closedColor = new Color(0.9f, 0.25f, 0.25f);
        [SerializeField] private Color openColor = new Color(0.35f, 1f, 0.55f);

        public bool IsOpen { get; private set; }

        private void Awake()
        {
            if (indicator != null) indicator.color = closedColor;
        }

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;

            if (barrier != null) barrier.SetActive(false);
            if (indicator != null) indicator.color = openColor;

            AudioClip clip = Resources.Load<AudioClip>("SFX/puerta");
            if (clip != null) Audio.AudioManager.PlaySfx(clip, 0.8f);
        }
    }
}
