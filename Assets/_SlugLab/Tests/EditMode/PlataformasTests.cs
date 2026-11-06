using NUnit.Framework;
using SlugLab.Level;
using SlugLab.Player;
using UnityEngine;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-41 y TC-43: cámara adaptable y entrada virtual.
    /// </summary>
    [TestFixture]
    public class PlataformasTests
    {
        [TearDown]
        public void TearDown() => VirtualInput.Reset();

        [Test]
        public void TC41_LaCamaraSeAjustaAlAnchoVisibleDeCadaPantalla()
        {
            const float ortho = 5.6f;
            const float levelMin = 0f;
            const float levelMax = 84f;

            float at16x9 = CameraFollow2D.ClampCenterX(100f, ortho, 16f / 9f, levelMin, levelMax);
            float at20x9 = CameraFollow2D.ClampCenterX(100f, ortho, 20f / 9f, levelMin, levelMax);

            Assert.AreEqual(levelMax - ortho * 16f / 9f, at16x9, 0.01f);
            Assert.Less(at20x9, at16x9, "En una pantalla más ancha la cámara debe detenerse antes.");
        }

        [Test]
        public void TC41b_EnUnNivelMasEstrechoQueLaPantallaLaCamaraSeCentra()
        {
            float center = CameraFollow2D.ClampCenterX(0f, 5.6f, 20f / 9f, 0f, 20f);

            Assert.AreEqual(10f, center, 0.01f, "Si el nivel cabe entero, la cámara se centra.");
        }

        [Test]
        public void TC43_LaEntradaVirtualRegistraUnaSolaPulsacion()
        {
            VirtualInput.SetJump(true);

            Assert.IsTrue(VirtualInput.JumpHeld);
            Assert.IsTrue(VirtualInput.ConsumeJumpPressed(), "La pulsación debe llegar una vez.");
            Assert.IsFalse(VirtualInput.ConsumeJumpPressed(), "No debe repetirse sin soltar.");

            VirtualInput.SetJump(false);
            VirtualInput.SetJump(true);
            Assert.IsTrue(VirtualInput.ConsumeJumpPressed(), "Un nuevo toque vuelve a registrarse.");
        }

        [Test]
        public void TC43b_ResetLimpiaTodo()
        {
            VirtualInput.SetMove(new Vector2(0.8f, -0.5f));
            VirtualInput.SetFire(true);
            VirtualInput.PressMelee();

            VirtualInput.Reset();

            Assert.AreEqual(0f, VirtualInput.Horizontal);
            Assert.AreEqual(0f, VirtualInput.Vertical);
            Assert.IsFalse(VirtualInput.FireHeld);
            Assert.IsFalse(VirtualInput.ConsumeMeleePressed());
        }
    }
}
