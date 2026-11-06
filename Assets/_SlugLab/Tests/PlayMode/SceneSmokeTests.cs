using System.Collections;
using System.Collections.Generic;
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
    /// TC-31 a TC-33: las escenas cargan, el jugador cae al suelo y el HUD existe.
    /// Son las pruebas de humo: si algo se rompe al regenerar escenas, saltan aquí.
    /// </summary>
    [TestFixture]
    public class SceneSmokeTests
    {
        private static readonly string[] Scenes =
        {
            "Menu", "Nivel1_Cantina", "Nivel2_Jefe", "Lab_01_Sprites", "Lab_02_Animacion",
            "Lab_03_Fisica", "Lab_04_Combate", "Lab_05_IA", "Lab_06_Niveles", "Lab_07_UI",
            "Lab_08_Jefe", "Lab_09_Bugs"
        };

        [UnityTest]
        public IEnumerator TC31_TodasLasEscenasCarganSinComponentesFaltantes()
        {
            foreach (string scene in Scenes)
            {
                SceneManager.LoadScene(scene);
                yield return null;
                yield return null;

                var broken = new List<string>();
                foreach (GameObject go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
                {
                    if (go.GetComponents<Component>().Any(component => component == null))
                        broken.Add(go.name);
                }

                Assert.IsEmpty(broken, $"La escena '{scene}' tiene scripts faltantes en: {string.Join(", ", broken)}");
            }
        }

        [UnityTest]
        public IEnumerator TC32_ElJugadorQuedaApoyadoEnElSuelo()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;

            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            Assert.IsNotNull(player, "Nivel1 debe tener un PlayerController.");

            yield return new WaitForSeconds(1.2f);

            Assert.IsTrue(player.IsGrounded, "El jugador debe estar en el suelo después de caer.");
            Assert.Greater(player.transform.position.y, -0.3f, "El jugador no debe atravesar el tilemap.");
        }

        [UnityTest]
        public IEnumerator TC33_ElNivelTieneHUD()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;

            Assert.IsNotNull(Object.FindFirstObjectByType<HUDController>(), "Falta el HUD.");
            Assert.IsNotNull(Object.FindFirstObjectByType<PauseMenu>(), "Falta el menú de pausa.");
        }
    }
}
