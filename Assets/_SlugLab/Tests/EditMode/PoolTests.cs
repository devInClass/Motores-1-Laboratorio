using NUnit.Framework;
using SlugLab.Core;
using SlugLab.Utils;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-14 a TC-18: pool de objetos y cooldowns (sin Unity, lógica pura).
    /// </summary>
    [TestFixture]
    public class PoolTests
    {
        [Test]
        public void TC14_Prewarm_CreatesObjectsUpFront()
        {
            var pool = new Pool<object>(() => new object(), prewarm: 3);

            Assert.AreEqual(3, pool.Created);
            Assert.AreEqual(3, pool.IdleCount);
            Assert.AreEqual(0, pool.ActiveCount);
        }

        [Test]
        public void TC15_Get_ReusesReleasedObject()
        {
            var pool = new Pool<object>(() => new object(), prewarm: 1);
            object first = pool.Get();

            pool.Release(first);
            object second = pool.Get();

            Assert.AreSame(first, second, "Debe reciclarse el mismo objeto.");
            Assert.AreEqual(1, pool.Created);
        }

        [Test]
        public void TC16_Get_WithoutIdle_CreatesNew()
        {
            var pool = new Pool<object>(() => new object(), prewarm: 1);
            pool.Get();
            pool.Get();

            Assert.AreEqual(2, pool.Created);
        }

        [Test]
        public void TC17_Release_Null_DoesNotThrow()
        {
            var pool = new Pool<object>(() => new object());
            Assert.DoesNotThrow(() => pool.Release(null));
        }

        [Test]
        public void TC18_Cooldown_BlocksUntilDurationPasses()
        {
            var cooldown = new Cooldown(0.5f);

            Assert.IsTrue(cooldown.TryConsume(0f), "La primera vez debe pasar.");
            Assert.IsFalse(cooldown.TryConsume(0.3f), "Antes de 0.5s debe bloquear.");
            Assert.IsTrue(cooldown.TryConsume(0.5f), "A los 0.5s debe volver a pasar.");
            Assert.AreEqual(0.3f, cooldown.Remaining(0.7f), 0.001f);
        }
    }
}
