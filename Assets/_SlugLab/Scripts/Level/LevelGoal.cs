using SlugLab.Core;
using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Meta del nivel. Al entrar el jugador se avanza de escena (o se gana la
    /// partida si es la última).
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LevelGoal : MonoBehaviour
    {
        [SerializeField] private bool requireBossDefeated;

        private void Awake() => GetComponent<Collider2D>().isTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (requireBossDefeated && !GameSession.BossDefeated) return;

            if (GameFlow.Instance != null) GameFlow.Instance.CompleteLevel();
        }
    }
}
