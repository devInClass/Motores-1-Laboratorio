using UnityEngine;

namespace SlugLab.Combat
{
    /// <summary>Bando al que pertenece una entidad. Sirve para no dañar a los aliados.</summary>
    public enum Team
    {
        Player = 0,
        Enemy = 1,
        Neutral = 2
    }

    /// <summary>
    /// Todo lo que necesitamos saber de un golpe: cuánto quita, dónde pega,
    /// hacia dónde empuja y quién lo lanzó.
    /// </summary>
    /// <remarks>
    /// Es un <c>struct</c> de solo lectura: se copia al pasarlo y nadie puede
    /// modificarlo por accidente a mitad de un combate.
    /// </remarks>
    public readonly struct DamageInfo
    {
        public readonly int Amount;
        public readonly Vector2 Point;
        public readonly Vector2 Direction;
        public readonly float Knockback;
        public readonly GameObject Source;

        public DamageInfo(int amount, Vector2 point, Vector2 direction, float knockback = 0f, GameObject source = null)
        {
            Amount = Mathf.Max(0, amount);
            Point = point;
            Direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector2.right;
            Knockback = Mathf.Max(0f, knockback);
            Source = source;
        }

        public static DamageInfo Simple(int amount, Vector2 point, Vector2 direction)
            => new DamageInfo(amount, point, direction);
    }

    /// <summary>Cualquier cosa que pueda recibir daño (jugador, enemigo, barril...).</summary>
    public interface IDamageable
    {
        /// <summary>Devuelve true solo si el daño se aplicó de verdad.</summary>
        bool ApplyDamage(DamageInfo info);

        Team Team { get; }

        bool IsAlive { get; }
    }
}
