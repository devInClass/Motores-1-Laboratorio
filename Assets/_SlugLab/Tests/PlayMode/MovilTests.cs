using System.Collections;
using System.Linq;
using NUnit.Framework;
using SlugLab.Core;
using SlugLab.Player;
using SlugLab.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace SlugLab.Tests.PlayMode
{
    /// <summary>
    /// TC-42 y TC-44: controles táctiles y área segura del HUD.
    /// </summary>
    [TestFixture]
    public class MovilTests
    {
        [TearDown]
        public void TearDown()
        {
            VirtualInput.Reset();
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }

        [UnityTest]
        public IEnumerator TC42_LosControlesTactilesSeConstruyenYResponden()
        {
            SceneManager.LoadScene("Lab_07_UI");
            yield return null;
            yield return null;

            MobileControls controls = Object.FindFirstObjectByType<MobileControls>();
            Assert.IsNotNull(controls, "Lab_07 debe traer los controles de prueba.");

            JoystickExists();

            TouchButton jump = Object.FindObjectsByType<TouchButton>(FindObjectsSortMode.None)
                .FirstOrDefault(button => button.name == "Saltar");
            Assert.IsNotNull(jump, "Falta el botón de salto.");

            VirtualInput.Reset();
            jump.OnPointerDown(null);

            Assert.IsTrue(VirtualInput.JumpHeld, "El botón debe mantener el salto.");
            Assert.IsTrue(VirtualInput.ConsumeJumpPressed(), "El botón debe registrar la pulsación.");

            jump.OnPointerUp(null);
            Assert.IsFalse(VirtualInput.JumpHeld, "Al soltar deja de saltar.");
        }

        private static void JoystickExists()
        {
            TouchJoystick joystick = Object.FindFirstObjectByType<TouchJoystick>();
            Assert.IsNotNull(joystick, "Falta el joystick táctil.");
        }

        [UnityTest]
        public IEnumerator TC44_ElHUDUsaAreaSegura()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;

            SafeAreaFitter fitter = Object.FindFirstObjectByType<SafeAreaFitter>();
            Assert.IsNotNull(fitter, "El HUD debe tener un contenedor de área segura para los notches.");
        }

        [UnityTest]
        public IEnumerator TC45_ElBotonDePantallaCompletaExisteYResponde()
        {
            SceneManager.LoadScene("Lab_07_UI");
            yield return null;
            yield return null;

            FullscreenToggle toggle = Object.FindFirstObjectByType<FullscreenToggle>();
            Assert.IsNotNull(toggle, "Falta el botón de pantalla completa.");

            Button button = toggle.GetComponentInChildren<Button>();
            Assert.IsNotNull(button, "El toggle debe tener un botón.");

            Assert.DoesNotThrow(() => button.onClick.Invoke(), "El clic no debe lanzar errores.");
            Assert.DoesNotThrow(() => button.onClick.Invoke(), "Debe poder alternarse dos veces.");
        }

        [UnityTest]
        public IEnumerator TC46_LaPausaAutomaticaDetieneElJuego()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;

            GameFlow flow = GameFlow.Instance;
            Assert.IsNotNull(flow, "Falta el GameFlow del nivel.");

            flow.PauseForFocusLoss(true);
            yield return null;

            Assert.IsTrue(flow.IsPaused, "Al perder el foco debe quedar pausado.");
            Assert.AreEqual(0f, Time.timeScale, 0.001f, "El tiempo debe detenerse.");
            Assert.IsTrue(AudioListener.pause, "El audio también se calla.");

            PauseMenu menu = Object.FindFirstObjectByType<PauseMenu>();
            Assert.IsNotNull(menu, "Debe mostrarse el menú de pausa.");
            menu.Resume();
            yield return null;

            Assert.IsFalse(flow.IsPaused, "Al continuar se reanuda.");
            Assert.AreEqual(1f, Time.timeScale, 0.001f);
            Assert.IsFalse(AudioListener.pause);
        }
    }
}
