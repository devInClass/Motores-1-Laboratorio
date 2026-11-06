using System;
using UnityEngine;

namespace SlugLab.Utils
{
    /// <summary>
    /// Enfriamiento (cooldown) reutilizable. Es una clase normal de C#, no un
    /// MonoBehaviour: se puede probar sin abrir una escena.
    /// </summary>
    /// <remarks>
    /// El tiempo se pasa como parámetro (<c>now</c>) en lugar de leer
    /// <c>Time.time</c> por dentro. Eso se llama "inyectar la dependencia" y es
    /// lo que permite escribir tests deterministas: en un test le pasamos 0, 0.5,
    /// 1.0... y comprobamos exactamente cuándo vuelve a estar listo.
    /// </remarks>
    [Serializable]
    public class Cooldown
    {
        [SerializeField] private float duration = 0.2f;

        private float _readyAtTime;

        public Cooldown() { }

        public Cooldown(float duration)
        {
            this.duration = Mathf.Max(0f, duration);
        }

        public float Duration
        {
            get => duration;
            set => duration = Mathf.Max(0f, value);
        }

        public bool IsReady(float now) => now >= _readyAtTime;

        /// <summary>Si está listo, lo consume y devuelve true. Si no, devuelve false.</summary>
        public bool TryConsume(float now)
        {
            if (!IsReady(now)) return false;
            _readyAtTime = now + duration;
            return true;
        }

        public float Remaining(float now) => Mathf.Max(0f, _readyAtTime - now);

        public void Reset() => _readyAtTime = 0f;
    }

    /// <summary>Utilidades de tiempo que usamos en varios sitios.</summary>
    public static class TimeUtil
    {
        /// <summary>Convierte "frames por segundo" de una animación en segundos.</summary>
        public static float FpsToSeconds(float fps) => fps <= 0f ? 0f : 1f / fps;
    }
}
