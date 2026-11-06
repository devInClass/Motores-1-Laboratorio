using SlugLab.Combat;
using SlugLab.Core;
using SlugLab.Player;
using UnityEngine;
using UnityEngine.UI;

namespace SlugLab.UI
{
    /// <summary>
    /// HUD del juego: vida, vidas restantes, puntos y munición.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerController player;
        [SerializeField] private PlayerCombat combat;

        [Header("Vida")]
        [SerializeField] private Image[] healthPips;
        [SerializeField] private Color fullColor = new Color(0.95f, 0.28f, 0.32f);
        [SerializeField] private Color emptyColor = new Color(0.22f, 0.15f, 0.18f);

        [Header("Textos")]
        [SerializeField] private Text livesText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text ammoText;

        private void Start()
        {
            if (player == null) player = FindFirstObjectByType<PlayerController>();
            if (combat == null && player != null) combat = player.GetComponent<PlayerCombat>();

            if (player != null && player.Damageable != null)
            {
                player.Damageable.Health.Changed += OnHealthChanged;
                OnHealthChanged(player.Damageable.Health.Current, player.Damageable.Health.Max);
            }

            GameSession.ScoreChanged += OnScoreChanged;
            GameSession.LivesChanged += OnLivesChanged;
            OnScoreChanged(GameSession.Score);
            OnLivesChanged(GameSession.Lives);
        }

        private void OnDestroy()
        {
            if (player != null && player.Damageable != null)
                player.Damageable.Health.Changed -= OnHealthChanged;

            GameSession.ScoreChanged -= OnScoreChanged;
            GameSession.LivesChanged -= OnLivesChanged;
        }

        private void Update()
        {
            if (ammoText == null || combat == null || combat.CurrentWeapon == null) return;

            Weapon weapon = combat.CurrentWeapon;
            ammoText.text = weapon.HasInfiniteAmmo
                ? "\u221E"
                : (weapon.IsReloading ? "..." : weapon.Ammo.ToString());
        }

        private void OnHealthChanged(int current, int max)
        {
            if (healthPips == null) return;

            for (int i = 0; i < healthPips.Length; i++)
            {
                if (healthPips[i] == null) continue;
                healthPips[i].color = i < current ? fullColor : emptyColor;
            }
        }

        private void OnScoreChanged(int score)
        {
            if (scoreText != null) scoreText.text = score.ToString("000000");
        }

        private void OnLivesChanged(int lives)
        {
            if (livesText != null) livesText.text = "x" + lives;
        }
    }
}
