using System.Collections;
using System.Linq;
using NUnit.Framework;
using SlugLab.Combat;
using SlugLab.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace SlugLab.Tests.PlayMode
{
    /// <summary>
    /// TC-37 y TC-39: regresiones de los dos bugs reportados en clase.
    /// </summary>
    [TestFixture]
    public class RegresionTests
    {
        [UnityTest]
        public IEnumerator TC37_LasBalasNoHeredanElMovimientoDelJugador()
        {
            SceneManager.LoadScene("Lab_03_Fisica");
            yield return null;

            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            Assert.IsNotNull(player, "Falta el jugador.");

            ProjectilePool pool = player.GetComponentInChildren<ProjectilePool>();
            Assert.IsNotNull(pool, "Falta el pool de balas del jugador.");

            Projectile bullet = pool.Get(new Vector2(2f, 1f), Quaternion.identity);
            bullet.Launch(Vector2.right, Team.Player, 1, 8f);

            float startX = bullet.transform.position.x;
            player.transform.position += Vector3.right * 3f;

            yield return new WaitForSeconds(0.3f);

            float travelled = bullet.transform.position.x - startX;

            // 8 u/s * 0.3 s = 2.4. Si la bala fuera hija del jugador avanzaría ~5.4.
            Assert.AreEqual(2.4f, travelled, 0.7f,
                "La bala debe avanzar solo con su propia velocidad, no arrastrada por el jugador.");
        }

        [UnityTest]
        public IEnumerator TC39_ElCartelDeInstruccionesSeDibujaEnOverlay()
        {
            SceneManager.LoadScene("Lab_03_Fisica");
            yield return null;

            Canvas[] canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            Canvas sign = canvases.FirstOrDefault(c => c.name == "Cartel_Instrucciones");

            Assert.IsNotNull(sign, "No existe el cartel de instrucciones.");
            Assert.AreEqual(RenderMode.ScreenSpaceOverlay, sign.renderMode,
                "El cartel debe ser Screen Space Overlay; en World Space queda detrás de los sprites.");

            Text[] texts = Object.FindObjectsByType<Text>(FindObjectsSortMode.None);
            Assert.IsTrue(texts.Any(t => t.name == "Mensaje" && !string.IsNullOrEmpty(t.text)),
                "El cartel debe tener texto visible.");
            Assert.IsTrue(texts.First(t => t.name == "Mensaje").font != null,
                "El texto necesita una fuente asignada.");
        }
    }
}
