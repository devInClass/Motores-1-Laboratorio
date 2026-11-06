using System;
using System.Collections.Generic;

namespace SlugLab.Core
{
    /// <summary>
    /// Pool genérico: reutiliza objetos en lugar de crearlos y destruirlos.
    /// Crear y destruir cientos de balas por segundo es caro; reutilizarlas no.
    /// </summary>
    /// <typeparam name="T">Cualquier objeto; no tiene que ser de Unity.</typeparam>
    public class Pool<T> where T : class
    {
        private readonly Func<T> _create;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly Stack<T> _idle = new Stack<T>();

        public int Created { get; private set; }
        public int IdleCount => _idle.Count;
        public int ActiveCount => Created - _idle.Count;

        public Pool(Func<T> create, Action<T> onGet = null, Action<T> onRelease = null, int prewarm = 0)
        {
            _create = create ?? throw new ArgumentNullException(nameof(create));
            _onGet = onGet;
            _onRelease = onRelease;

            for (int i = 0; i < prewarm; i++)
            {
                T item = CreateNew();
                _onRelease?.Invoke(item);
                _idle.Push(item);
            }
        }

        private T CreateNew()
        {
            Created++;
            return _create();
        }

        public T Get()
        {
            T item = _idle.Count > 0 ? _idle.Pop() : CreateNew();
            _onGet?.Invoke(item);
            return item;
        }

        public void Release(T item)
        {
            if (item == null) return;
            _onRelease?.Invoke(item);
            _idle.Push(item);
        }

        /// <summary>Vacía el pool. El <c>onRelease</c> se encarga de destruir si hace falta.</summary>
        public void Clear()
        {
            while (_idle.Count > 0) _onRelease?.Invoke(_idle.Pop());
            Created = 0;
        }
    }
}
