using SlugLab.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SlugLab.UI
{
    /// <summary>
    /// Pantalla final: misión cumplida o fallida, con puntos y botones.
    /// </summary>
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Text titleText;
        [SerializeField] private Text detailText;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button menuButton;

        private void Awake()
        {
            if (panel != null) panel.SetActive(false);

            if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
            if (menuButton != null) menuButton.onClick.AddListener(OnMenu);
        }

        public void ShowGameOver()
        {
            Show("MISION FALLIDA", "Puntos: " + GameSession.Score + "\nLa cantina sigue en manos del enemigo.");
        }

        public void ShowVictory()
        {
            Show("MISION CUMPLIDA", "Puntos: " + GameSession.Score + "\nDerrotaste al jefe final.");
        }

        private void Show(string title, string detail)
        {
            if (panel != null) panel.SetActive(true);
            if (titleText != null) titleText.text = title;
            if (detailText != null) detailText.text = detail;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
