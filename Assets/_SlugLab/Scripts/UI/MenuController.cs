using SlugLab.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SlugLab.UI
{
    /// <summary>Botones del menú principal.</summary>
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private string firstLevelScene = "Nivel1_Cantina";
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            if (playButton != null) playButton.onClick.AddListener(Play);
            if (quitButton != null) quitButton.onClick.AddListener(Quit);
        }

        public void Play()
        {
            GameSession.Reset();
            Time.timeScale = 1f;
            SceneManager.LoadScene(firstLevelScene);
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
