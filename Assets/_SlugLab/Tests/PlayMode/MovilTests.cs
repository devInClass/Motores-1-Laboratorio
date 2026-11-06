using System.Collections;
using System.Linq;
using NUnit.Framework;
using SlugLab.Player;
using SlugLab.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SlugLab.Tests.PlayMode
{
    /// <summary>
    /// TC-42 y TC-44: controles táctiles y área segura del HUD.
    /// </summary>
    [TestFixture]
    public class MovilTests
    {
        [TearDown]
        public void TearDown() => VirtualInput.Reset();

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
    }
}
