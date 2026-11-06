using SlugLab.Boss;
using UnityEngine;
using UnityEngine.UI;

namespace SlugLab.UI
{
    /// <summary>
    /// Barra de vida del jefe. Se muestra solo cuando el jefe existe en la escena.
    /// </summary>
    public class BossHealthBar : MonoBehaviour
    {
        [SerializeField] private BossController boss;
        [SerializeField] private GameObject root;
        [SerializeField] private Image fill;
        [SerializeField] private Text bossName;
        [SerializeField] private Text phaseLabel;

        private void Start()
        {
            if (boss == null) boss = FindFirstObjectByType<BossController>();

            if (boss == null)
            {
                if (root != null) root.SetActive(false);
                return;
            }

            if (root != null) root.SetActive(true);
            if (bossName != null) bossName.text = boss.DisplayName;

            boss.HealthChanged += OnHealthChanged;
            OnHealthChanged(boss.HealthNormalized);
        }

        private void OnDestroy()
        {
            if (boss != null) boss.HealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float normalized)
        {
            if (fill != null) fill.fillAmount = Mathf.Clamp01(normalized);
            if (phaseLabel != null && boss != null) phaseLabel.text = "FASE " + (boss.CurrentPhaseIndex + 1);
        }
    }
}
