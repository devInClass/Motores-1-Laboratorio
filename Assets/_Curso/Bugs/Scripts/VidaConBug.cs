using System;

namespace SlugLab.Curso.Bugs
{
    /// <summary>
    /// ESTACION 1 - La vida que sube cuando te golpean.
    ///
    /// Sintomas: al recibir dano el jugador se cura en lugar de perder vida.
    /// Pista: revisa el signo de la operacion y el limite inferior.
    /// </summary>
    public class VidaConBug
    {
        public int Max { get; }
        public int Actual { get; private set; }

        public VidaConBug(int max)
        {
            Max = max;
            Actual = max;
        }

        public bool RecibirDano(int cantidad)
        {
            if (cantidad <= 0) return false;

            // BUG: esta linea esta mal. Corrigela.
            Actual = Actual + cantidad;

            if (Actual > Max) Actual = Max;
            return true;
        }
    }
}
