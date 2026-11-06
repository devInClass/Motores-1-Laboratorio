using UnityEngine;

namespace SlugLab.Enemies
{
    /// <summary>
    /// Parámetros de un tipo de enemigo. Crear variantes es tan fácil como
    /// duplicar este asset: soldado rápido, tanque lento, francotirador...
    /// </summary>
    [CreateAssetMenu(fileName = "ConfiguracionEnemigo", menuName = "SlugLab/Enemigos/Configuracion", order = 1)]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Vida y puntos")]
        public int maxHealth = 3;
        public int scoreValue = 100;

        [Header("Movimiento")]
        public float patrolSpeed = 1.6f;
        public float chaseSpeed = 3.2f;
        public float patrolDistance = 4f;

        [Header("Detección")]
        public float sightRange = 9f;
        public float loseSightRange = 12f;
        public float attackRange = 8f;

        [Header("Combate")]
        public float attackInterval = 1.5f;
        public float alertTime = 0.35f;
        public int projectileDamage = 1;
        public float projectileSpeed = 11f;

        [Header("Muerte")]
        public float deathDelay = 0.6f;
    }
}
