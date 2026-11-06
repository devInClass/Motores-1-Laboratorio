using SlugLab.Core;
using SlugLab.Player;
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
            bool pressed = Input.GetKeyDown(KeyCode.Escape) || VirtualInput.ConsumePausePressed();
            if (pressed && !IsGameOver()) Toggle();
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
            AudioListener.pause = open;
        }

        /// <summary>
        /// Pausa desde código (por ejemplo cuando Android manda la app al fondo).
        /// No alterna: si ya está pausado no hace nada.
        /// </summary>
        public void Pause()
        {
            if (GameFlow.Instance == null) return;
            if (GameFlow.Instance.IsPaused || GameFlow.Instance.IsGameOver || GameFlow.Instance.IsVictory) return;

            GameFlow.Instance.SetPaused(true);
            if (panel != null) panel.SetActive(true);
            Cursor.visible = true;
            AudioListener.pause = true;
        }

        public void Resume()
        {
            if (GameFlow.Instance == null) return;
            GameFlow.Instance.SetPaused(false);
            if (panel != null) panel.SetActive(false);
            Cursor.visible = false;
            AudioListener.pause = false;
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
