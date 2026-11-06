using SlugLab.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SlugLab.UI
{
    /// <summary>
    /// Menú de pausa con Escape. Detiene el tiempo del juego (timeScale = 0)
    /// pero la interfaz sigue respondiendo.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);

            if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
            if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
            if (menuButton != null) menuButton.onClick.AddListener(OnMenu);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !IsGameOver()) Toggle();
        }

        private static bool IsGameOver()
        {
            GameFlow flow = GameFlow.Instance;
            return flow != null && (flow.IsGameOver || flow.IsVictory);
        }

        public void Toggle()
        {
            if (GameFlow.Instance == null) return;

            bool open = !GameFlow.Instance.IsPaused;
            GameFlow.Instance.SetPaused(open);

            if (panel != null) panel.SetActive(open);

            Cursor.visible = open;
        }

        public void Resume()
        {
            if (GameFlow.Instance == null) return;
            GameFlow.Instance.SetPaused(false);
            if (panel != null) panel.SetActive(false);
            Cursor.visible = false;
        }

        private void OnRestart()
        {
            if (GameFlow.Instance != null) GameFlow.Instance.RestartLevel();
        }

        private void OnMenu()
        {
            if (GameFlow.Instance != null) GameFlow.Instance.GoToMenu();
        }
    }
}
