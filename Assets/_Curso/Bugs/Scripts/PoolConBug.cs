using System;
using System.Collections.Generic;

namespace SlugLab.Curso.Bugs
{
    /// <summary>
    /// ESTACION 3 - El pool que no recicla nada.
    ///
    /// Sintomas: cada bala crea un objeto nuevo; el contador de creados sube
    /// sin parar aunque se liberen objetos.
    /// Pista: revisa que hace Liberar con el objeto.
    /// </summary>
    public class PoolConBug<T> where T : class
    {
        private readonly Func<T> _crear;
        private readonly List<T> _libres = new List<T>();

        public int Creados { get; private set; }
        public int Libres => _libres.Count;

        public PoolConBug(Func<T> crear)
        {
            _crear = crear;
        }

        public T Obtener()
        {
            Creados++;
            return _crear();
        }

        public void Liberar(T objeto)
        {
            // BUG: el objeto liberado se pierde. Corrigelo.
        }
    }
}
