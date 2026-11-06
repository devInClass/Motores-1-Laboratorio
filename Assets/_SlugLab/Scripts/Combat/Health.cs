using System;
using UnityEngine;

namespace SlugLab.Combat
{
    /// <summary>
    /// Vida de una entidad. Clase pura de C#: no hereda de MonoBehaviour y por
    /// eso se puede probar en un test sin escena, sin esperar frames.
    /// </summary>
    public class Health
    {
        public int Max { get; }
        public int Current { get; private set; }
        public bool IsDead => Current <= 0;
        public float Normalized => Max <= 0 ? 0f : (float)Current / Max;

        /// <summary>(actual, máximo)</summary>
        public event Action<int, int> Changed;
        public event Action<DamageInfo> Damaged;
        public event Action Died;

        public Health(int max)
        {
            Max = Mathf.Max(1, max);
            Current = Max;
        }

        /// <summary>Aplica daño. Devuelve false si ya estaba muerto o el daño era 0.</summary>
        public bool ApplyDamage(DamageInfo info)
        {
            if (IsDead || info.Amount <= 0) return false;

            Current = Mathf.Max(0, Current - info.Amount);
            Damaged?.Invoke(info);
            Changed?.Invoke(Current, Max);

            if (Current == 0) Died?.Invoke();
            return true;
        }

        public void Heal(int amount)
        {
            if (amount <= 0 || IsDead) return;
            Current = Mathf.Min(Max, Current + amount);
            Changed?.Invoke(Current, Max);
        }

        public void ResetToFull()
        {
            Current = Max;
            Changed?.Invoke(Current, Max);
        }
    }
}
