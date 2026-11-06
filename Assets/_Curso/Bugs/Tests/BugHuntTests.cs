using NUnit.Framework;
using SlugLab.Curso.Bugs;

namespace SlugLab.Curso.Bugs.Tests
{
    /// <summary>
    /// Tests de la caceria de bugs. Solo se compilan si activas el define
    /// BUG_HUNT en Project Settings > Player > Scripting Define Symbols.
    ///
    /// Mientras los bugs no esten corregidos, estos tests FALLAN: esa es la
    /// senal de que te falta trabajo, no un error del repositorio.
    /// </summary>
    [TestFixture]
    public class BugHuntTests
    {
        [Test]
        public void BUG01_LaVidaBajaAlRecibirDano()
        {
            var vida = new VidaConBug(5);

            vida.RecibirDano(2);

            Assert.AreEqual(3, vida.Actual, "5 - 2 debe ser 3.");
        }

        [Test]
        public void BUG02_ElArmaRespetaLaCadencia()
        {
            var arma = new ArmaConBug { Intervalo = 0.12f };

            Assert.IsTrue(arma.Disparar(0f), "El primer disparo debe salir.");
            Assert.IsFalse(arma.Disparar(0.05f), "A 0.05 s no debe disparar.");
            Assert.IsTrue(arma.Disparar(0.12f), "A 0.12 s ya debe disparar.");
        }

        [Test]
        public void BUG03_ElPoolReciclaObjetos()
        {
            var pool = new PoolConBug<object>(() => new object());

            object primero = pool.Obtener();
            pool.Liberar(primero);
            pool.Obtener();

            Assert.AreEqual(1, pool.Creados, "El segundo objeto debe reutilizar el primero.");
        }

        [Test]
        public void BUG04_ElMarcadorSumaPuntos()
        {
            var contador = new ContadorConBug();

            contador.Sumar(100);
            contador.Sumar(250);

            Assert.AreEqual(350, contador.Total);
        }
    }
}
