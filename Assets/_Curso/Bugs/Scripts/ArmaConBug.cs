namespace SlugLab.Curso.Bugs
{
    /// <summary>
    /// ESTACION 2 - El arma que dispara sin parar.
    ///
    /// Sintomas: se puede disparar todos los frames y la cadencia no se respeta.
    /// Pista: falta comprobar el tiempo del siguiente disparo antes de disparar.
    /// </summary>
    public class ArmaConBug
    {
        public float Intervalo = 0.12f;
        public float SiguienteDisparoPermitido { get; private set; }

        public bool Disparar(float ahora)
        {
            // BUG: falta la comprobacion de cadencia. Corrigela.
            SiguienteDisparoPermitido = ahora + Intervalo;
            return true;
        }
    }
}
