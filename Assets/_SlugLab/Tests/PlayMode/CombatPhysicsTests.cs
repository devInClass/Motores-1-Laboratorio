using System.Collections;
using NUnit.Framework;
using SlugLab.Combat;
using SlugLab.Player;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace SlugLab.Tests.PlayMode
{
    /// <summary>
    /// TC-34 a TC-36: física del jugador, daño del proyectil y patrulla enemiga.
    /// </summary>
    [TestFixture]
    public class CombatPhysicsTests
    {
        [UnityTest]
        public IEnumerator TC34_LaGravedadHaceCaerAlJugadorYElSaltoLoSube()
        {
            SceneManager.LoadScene("Nivel1_Cantina");
            yield return null;

            PlayerController player = Object.FindFirstObjectByType<PlayerController>();
            Assert.IsNotNull(player);

            yield return new WaitForSeconds(1f);
            float groundY = player.transform.position.y;
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();

            body.linearVelocity = new Vector2(0f, 12f);
            yield return new WaitForSeconds(0.2f);

            Assert.Greater(player.transform.position.y, groundY + 0.5f, "El salto debe elevar al jugador.");
            Assert.IsFalse(player.IsGrounded, "En el aire no debe considerarse en el suelo.");

            yield return new WaitForSeconds(1.5f);
            Assert.IsTrue(player.IsGrounded, "Debe volver a caer y quedar en el suelo.");
        }

        [UnityTest]
        public IEnumerator TC35_LaBalaDelJugadorDaniaAlEnemigo()
        {
            SceneManager.LoadScene("Lab_03_Fisica");
            yield return null;

            GameObject enemyPrefab = PrefabTestUtility.Load("Enemigo_Soldado");
            GameObject bulletPrefab = PrefabTestUtility.Load("Bala_Jugador");
            Assert.IsNotNull(enemyPrefab, "Falta el prefab del enemigo.");
            Assert.IsNotNull(bulletPrefab, "Falta el prefab de la bala.");

            GameObject enemy = Object.Instantiate(enemyPrefab, new Vector3(14f, 0.2f, 0f), Quaternion.identity);
            Damageable damageable = enemy.GetComponent<Damageable>();
            Assert.IsNotNull(damageable);

            yield return null;
            int before = damageable.Health.Current;

            GameObject bulletObject = Object.Instantiate(bulletPrefab, new Vector3(10f, 0.8f, 0f), Quaternion.identity);
            Projectile projectile = bulletObject.GetComponent<Projectile>();
            Assert.IsNotNull(projectile);
            projectile.Launch(Vector2.right, Team.Player, 1, 20f);

            yield return new WaitForSeconds(0.4f);

            Assert.Less(damageable.Health.Current, before, "La bala del jugador debe dañar al enemigo.");
        }

        [UnityTest]
        public IEnumerator TC36_ElEnemigoPatrulla()
        {
            SceneManager.LoadScene("Lab_03_Fisica");
            yield return null;

            GameObject prefab = PrefabTestUtility.Load("Enemigo_Soldado");
            Assert.IsNotNull(prefab, "Falta el prefab Enemigo_Soldado.");

            GameObject enemy = Object.Instantiate(prefab, new Vector3(12f, 0.2f, 0f), Quaternion.identity);
            yield return null;

            Vector3 start = enemy.transform.position;
            yield return new WaitForSeconds(1f);

            Assert.Greater(Mathf.Abs(enemy.transform.position.x - start.x), 0.15f, "El enemigo debe patrullar.");
        }
    }
}
