using NUnit.Framework;
using SlugLab.Combat;
using UnityEngine;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-07 a TC-13: cadencia, munición y recarga del arma.
    /// </summary>
    [TestFixture]
    public class WeaponTests
    {
        private WeaponDefinition _definition;

        [SetUp]
        public void SetUp()
        {
            _definition = ScriptableObject.CreateInstance<WeaponDefinition>();
            _definition.damage = 1;
            _definition.fireInterval = 0.12f;
            _definition.magazineSize = 3;
            _definition.reloadTime = 1f;
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_definition);
        }

        [Test]
        public void TC07_FirstShot_IsAllowed()
        {
            var weapon = new Weapon(_definition);
            Assert.IsTrue(weapon.TryFire(10f));
        }

        [Test]
        public void TC08_FireRate_BlocksShotsDuringCooldown()
        {
            var weapon = new Weapon(_definition);

            Assert.IsTrue(weapon.TryFire(10f), "El primer disparo debe pasar.");
            Assert.IsFalse(weapon.TryFire(10.05f), "0.05s < 0.12s: debe bloquearse.");
            Assert.IsTrue(weapon.TryFire(10.12f), "Al cumplir la cadencia debe permitirse.");
        }

        [Test]
        public void TC09_EachShot_ConsumesAmmo()
        {
            var weapon = new Weapon(_definition);

            weapon.TryFire(0f);
            weapon.TryFire(0.2f);

            Assert.AreEqual(1, weapon.Ammo);
        }

        [Test]
        public void TC10_EmptyMagazine_StartsReload()
        {
            var weapon = new Weapon(_definition);
            weapon.TryFire(0f);
            weapon.TryFire(0.2f);
            weapon.TryFire(0.4f);

            Assert.AreEqual(0, weapon.Ammo);
            Assert.IsTrue(weapon.IsReloading);
            Assert.IsFalse(weapon.CanFire(0.5f), "Recargando no se puede disparar.");
        }

        [Test]
        public void TC11_Reload_FinishesAfterReloadTime()
        {
            var weapon = new Weapon(_definition);
            weapon.TryFire(0f);
            weapon.TryFire(0.2f);
            weapon.TryFire(0.4f);

            weapon.Tick(1.3f); // la recarga termina en 0.4 + 1.0 = 1.4 -> aún no
            Assert.IsTrue(weapon.IsReloading, "Todavía no pasó el tiempo de recarga.");

            weapon.Tick(1.5f);
            Assert.IsFalse(weapon.IsReloading);
            Assert.AreEqual(3, weapon.Ammo);
        }

        [Test]
        public void TC12_InfiniteAmmo_NeverReloads()
        {
            _definition.magazineSize = 0;
            var weapon = new Weapon(_definition);

            for (int i = 0; i < 50; i++) weapon.TryFire(i * 0.5f);

            Assert.IsTrue(weapon.HasInfiniteAmmo);
            Assert.IsFalse(weapon.IsReloading);
        }

        [Test]
        public void TC13_Refill_RestoresMagazine()
        {
            var weapon = new Weapon(_definition);
            weapon.TryFire(0f);

            weapon.Refill();

            Assert.AreEqual(3, weapon.Ammo);
            Assert.IsFalse(weapon.IsReloading);
        }
    }
}
