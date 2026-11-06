using NUnit.Framework;
using SlugLab.Combat;
using UnityEngine;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-01 a TC-06: vida, daño, curación y muerte.
    /// </summary>
    [TestFixture]
    public class HealthTests
    {
        [Test]
        public void TC01_Damage_ReducesCurrent()
        {
            var health = new Health(5);

            bool applied = health.ApplyDamage(new DamageInfo(2, Vector2.zero, Vector2.right));

            Assert.IsTrue(applied, "El daño debería aplicarse.");
            Assert.AreEqual(3, health.Current);
            Assert.AreEqual(5, health.Max);
            Assert.IsFalse(health.IsDead);
        }

        [Test]
        public void TC02_Damage_ToZero_RaisesDiedExactlyOnce()
        {
            var health = new Health(2);
            int deaths = 0;
            health.Died += () => deaths++;

            health.ApplyDamage(new DamageInfo(5, Vector2.zero, Vector2.right));
            health.ApplyDamage(new DamageInfo(5, Vector2.zero, Vector2.right));

            Assert.AreEqual(0, health.Current, "La vida nunca debe quedar negativa.");
            Assert.AreEqual(1, deaths, "El evento Died debe dispararse una sola vez.");
            Assert.IsTrue(health.IsDead);
        }

        [Test]
        public void TC03_Damage_AfterDeath_ReturnsFalse()
        {
            var health = new Health(1);
            health.ApplyDamage(new DamageInfo(1, Vector2.zero, Vector2.right));

            bool applied = health.ApplyDamage(new DamageInfo(1, Vector2.zero, Vector2.right));

            Assert.IsFalse(applied);
        }

        [Test]
        public void TC04_Heal_DoesNotExceedMax()
        {
            var health = new Health(5);
            health.ApplyDamage(new DamageInfo(4, Vector2.zero, Vector2.right));

            health.Heal(99);

            Assert.AreEqual(5, health.Current);
        }

        [Test]
        public void TC05_ZeroDamage_IsIgnored()
        {
            var health = new Health(3);
            bool damagedEvent = false;
            health.Changed += (_, __) => damagedEvent = true;

            bool applied = health.ApplyDamage(new DamageInfo(0, Vector2.zero, Vector2.right));

            Assert.IsFalse(applied);
            Assert.IsFalse(damagedEvent);
            Assert.AreEqual(3, health.Current);
        }

        [Test]
        public void TC06_Normalized_ReflectsCurrentHealth()
        {
            var health = new Health(4);
            health.ApplyDamage(new DamageInfo(1, Vector2.zero, Vector2.right));

            Assert.AreEqual(0.75f, health.Normalized, 0.001f);
        }
    }
}
