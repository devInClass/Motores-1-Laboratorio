namespace SlugLab.Curso.Bugs
{
    /// <summary>
    /// ESTACION 4 - El marcador que resta puntos.
    ///
    /// Sintomas: al matar enemigos el puntaje baja en vez de subir.
    /// Pista: revisa el operador de la suma.
    /// </summary>
    public class ContadorConBug
    {
        public int Total { get; private set; }

        public void Sumar(int puntos)
        {
            // BUG: el signo esta mal. Corrigelo.
            Total = Total - puntos;
            if (Total < 0) Total = 0;
        }
    }
}
