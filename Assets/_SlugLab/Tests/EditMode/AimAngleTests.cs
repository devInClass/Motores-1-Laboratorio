using NUnit.Framework;
using SlugLab.Player;
using UnityEngine;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-38: el ángulo de apuntado que alimenta el blend tree 2D.
    /// </summary>
    [TestFixture]
    public class AimAngleTests
    {
        [Test]
        public void TC38_ElAnguloDeApuntadoCubreLasCincoDirecciones()
        {
            Assert.AreEqual(90f, PlayerCombat.AimAngleDegrees(Vector2.up), 0.01f);
            Assert.AreEqual(45f, PlayerCombat.AimAngleDegrees(new Vector2(1f, 1f)), 0.01f);
            Assert.AreEqual(0f, PlayerCombat.AimAngleDegrees(Vector2.right), 0.01f);
            Assert.AreEqual(0f, PlayerCombat.AimAngleDegrees(Vector2.left), 0.01f, "Mirar a la izquierda no cambia el ángulo: lo resuelve el volteo del sprite.");
            Assert.AreEqual(-45f, PlayerCombat.AimAngleDegrees(new Vector2(1f, -1f)), 0.01f);
            Assert.AreEqual(-90f, PlayerCombat.AimAngleDegrees(Vector2.down), 0.01f);
        }

        [Test]
        public void TC38b_ElAnguloSeMantieneEntreMenos90Y90()
        {
            for (int i = 0; i < 360; i += 15)
            {
                float radians = i * Mathf.Deg2Rad;
                var direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
                float angle = PlayerCombat.AimAngleDegrees(direction);

                Assert.GreaterOrEqual(angle, -90.01f);
                Assert.LessOrEqual(angle, 90.01f);
            }
        }
    }
}
