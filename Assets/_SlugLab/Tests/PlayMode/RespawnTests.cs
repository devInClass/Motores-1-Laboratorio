using System.Collections;
using NUnit.Framework;
using SlugLab.Combat;
using SlugLab.Core;
using SlugLab.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SlugLab.Tests.PlayMode
{
    /// <summary>
    /// TC-47 a TC-49: reaparición tras caer a un precipicio y checkpoints entre niveles.
    /// </summary>
    [TestFixture]
    public class RespawnTests
    {
        [SetUp]
        public void SetUp()
        {
            GameSession.Reset();
            Time.timeScale = 1f;
        }

        [TearDown]
        public void TearDown()
        {
            GameSession.Reset();
            Time.timeScale = 1f;
        }

        [UnityTest]
        public IEnumerator TC47_AlMorirSinCheckpointReapareceEnElInicio()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;
            yield return null;

            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            Assert.IsNotNull(player, "Falta el jugador.");

            Vector3 start = player.transform.position;

            // Simula la caída al precipicio y el daño letal de la zona de muerte.
            player.transform.position = new Vector3(21f, -9f, 0f);
            yield return null;
            player.Damageable.ApplyDamage(new DamageInfo(999, player.transform.position, Vector2.up, 0f, null));

            yield return new WaitForSeconds(4f);

            Assert.IsTrue(player.IsAlive, "El jugador debe revivir.");
            Assert.Less(Vector2.Distance(player.transform.position, start), 2.5f,
                "Sin checkpoint debe reaparecer en el inicio del nivel, no en el fondo del hueco.");
            Assert.Greater(player.transform.position.y, 0f, "No debe quedar debajo del suelo.");
        }

        [UnityTest]
        public IEnumerator TC48_ElCheckpointNoSeHeredaEntreNiveles()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;

            GameSession.SetCheckpoint(new Vector2(34f, 0.6f));
            Assert.IsTrue(GameSession.HasCheckpoint, "El checkpoint debería guardarse en este nivel.");

            SceneManager.LoadScene("Nivel2_Jefe");
            yield return null;

            Assert.IsFalse(GameSession.HasCheckpoint,
                "Al cambiar de nivel no se debe heredar el checkpoint anterior.");
        }

        [UnityTest]
        public IEnumerator TC49_ConCheckpointReapareceEnElCheckpoint()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;

            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            Assert.IsNotNull(player);

            Vector2 checkpoint = new Vector2(34f, 0.6f);
            GameSession.SetCheckpoint(checkpoint);

            player.transform.position = new Vector3(21f, -9f, 0f);
            yield return null;
            player.Damageable.ApplyDamage(new DamageInfo(999, player.transform.position, Vector2.up, 0f, null));

            yield return new WaitForSeconds(4f);

            Assert.Less(Vector2.Distance(player.transform.position, checkpoint), 2.5f,
                "Con checkpoint debe reaparecer en el checkpoint, no en el inicio.");
        }
    }
}
