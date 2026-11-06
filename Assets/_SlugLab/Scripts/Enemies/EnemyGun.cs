using SlugLab.Combat;
using UnityEngine;

namespace SlugLab.Enemies
{
    /// <summary>
    /// Dispara proyectiles enemigos. Separado del cerebro para poder reutilizarlo
    /// en torretas, jefes o enemigos voladores.
    /// </summary>
    public class EnemyGun : MonoBehaviour
    {
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private Transform muzzle;
        [SerializeField] private int damage = 1;
        [SerializeField] private float projectileSpeed = 11f;
        [SerializeField] private float spreadDegrees;

        public Vector3 MuzzlePosition => muzzle != null ? muzzle.position : transform.position;

        public void Fire(Vector2 direction)
        {
            if (projectilePool == null) return;

            if (Mathf.Abs(spreadDegrees) > 0.01f)
            {
                direction = (Vector2)(Quaternion.Euler(0f, 0f, Random.Range(-spreadDegrees, spreadDegrees)) * direction);
            }

            Projectile projectile = projectilePool.Get(MuzzlePosition, Quaternion.identity);
            projectile.Launch(direction, Team.Enemy, damage, projectileSpeed);
        }
    }
}
