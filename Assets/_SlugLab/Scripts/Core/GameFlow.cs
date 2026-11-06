using System.Collections;
using SlugLab.Level;
using SlugLab.Player;
using SlugLab.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlugLab.Core
{
    /// <summary>
    /// Director del juego: pausa, muerte, reaparición en checkpoint, cambio de
    /// nivel y final de partida. Solo hay uno por escena.
    /// </summary>
    public class GameFlow : MonoBehaviour
    {
        public static GameFlow Instance { get; private set; }

        [Header("Escenas")]
        [SerializeField] private string menuSceneName = "Menu";
        [Tooltip("Escena que se carga al terminar este nivel. Vacío = fin del juego.")]
        [SerializeField] private string nextSceneName = "";

        [Header("Puntos")]
        [SerializeField] private int scorePerLevel = 1000;
        [SerializeField] private int scorePerBoss = 5000;

        [Header("Reaparición")]
        [SerializeField] private float respawnDelay = 1.4f;

        [Header("Pausa automática")]
        [Tooltip("En Android y WebGL, pausa cuando la app pierde el foco.")]
        [SerializeField] private bool pauseOnFocusLoss = true;

        public bool IsPaused { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsVictory { get; private set; }

        private PlayerController _player;
        private PauseMenu _pauseMenu;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
            AudioListener.pause = false;

            _player = FindFirstObjectByType<PlayerController>();
            if (_player != null) _player.Died += HandlePlayerDied;
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) PauseForFocusLoss();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus) PauseForFocusLoss();
        }

        /// <summary>
        /// Pausa cuando la app va al fondo (Android) o la pestaña deja de estar
        /// visible (WebGL). Al volver, el jugador decide cuándo continuar.
        /// </summary>
        public void PauseForFocusLoss(bool ignorePlatform = false)
        {
            if (!pauseOnFocusLoss) return;
            if (!ignorePlatform &&
                !Application.isMobilePlatform &&
                Application.platform != RuntimePlatform.WebGLPlayer) return;

            if (IsGameOver || IsVictory || IsPaused) return;

            if (_pauseMenu == null) _pauseMenu = FindFirstObjectByType<PauseMenu>();
            if (_pauseMenu != null) _pauseMenu.Pause();
        }

        private void OnDestroy()
        {
            if (_player != null) _player.Died -= HandlePlayerDied;
            if (Instance == this) Instance = null;
        }

        public void SetPaused(bool paused)
        {
            if (IsGameOver || IsVictory) return;
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
        }

        public void TogglePause() => SetPaused(!IsPaused);

        private void HandlePlayerDied()
        {
            if (IsGameOver || IsVictory) return;

            if (GameSession.LoseLife())
            {
                StartCoroutine(RespawnRoutine());
            }
            else
            {
                TriggerGameOver();
            }
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnDelay);

            if (_player == null)
            {
                _player = FindFirstObjectByType<PlayerController>();
                if (_player == null) yield break;
                _player.Died += HandlePlayerDied;
            }

            Vector2 position = GameSession.HasCheckpoint ? GameSession.Checkpoint : (Vector2)_player.transform.position;
            _player.RespawnAt(position);

            if (CameraFollow2D.Current != null) CameraFollow2D.Current.SnapToTarget();
        }

        public void CompleteLevel()
        {
            if (IsVictory || IsGameOver) return;

            GameSession.AddScore(scorePerLevel);

            if (string.IsNullOrEmpty(nextSceneName))
            {
                Win();
                return;
            }

            Time.timeScale = 1f;
            GameSession.SetLevel(SceneManager.GetActiveScene().buildIndex + 1);
            SceneManager.LoadScene(nextSceneName);
        }

        public void Win()
        {
            if (IsVictory) return;
            IsVictory = true;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            GameSession.AddScore(scorePerBoss);

            EndScreen screen = FindFirstObjectByType<EndScreen>(FindObjectsInactive.Include);
            if (screen != null) screen.ShowVictory();
        }

        public void TriggerGameOver()
        {
            if (IsGameOver) return;
            IsGameOver = true;
            Time.timeScale = 1f;
            AudioListener.pause = false;

            EndScreen screen = FindFirstObjectByType<EndScreen>(FindObjectsInactive.Include);
            if (screen != null) screen.ShowGameOver();
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            GameSession.Reset();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(menuSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
