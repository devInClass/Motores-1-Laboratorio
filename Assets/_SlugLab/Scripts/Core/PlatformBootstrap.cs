using SlugLab.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SlugLab.Core
{
    /// <summary>
    /// Ajustes que solo tienen sentido en un dispositivo: límite de FPS, vsync
    /// y que la pantalla no se apague. En el editor no hace nada.
    /// </summary>
    public static class PlatformBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Configure()
        {
            VirtualInput.Reset();
            SceneManager.sceneLoaded += (_, __) => VirtualInput.Reset();

            if (!Application.isMobilePlatform) return;

            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = true;
            Screen.autorotateToLandscapeRight = true;
        }
    }
}
