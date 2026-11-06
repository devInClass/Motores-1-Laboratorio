using UnityEngine;

namespace SlugLab.Combat
{
    /// <summary>
    /// Datos de un arma. Al ser un ScriptableObject se puede crear desde el
    /// menú <c>Create &gt; SlugLab &gt; Armas</c> y reutilizar en varios prefabs
    /// sin duplicar valores.
    /// </summary>
    [CreateAssetMenu(fileName = "Arma", menuName = "SlugLab/Armas/Definicion de arma", order = 0)]
    public class WeaponDefinition : ScriptableObject
    {
        [Header("Identidad")]
        public string displayName = "Ametralladora";

        [Header("Daño")]
        public int damage = 1;
        public int projectilesPerShot = 1;
        [Range(0f, 45f)] public float spreadDegrees = 0f;

        [Header("Cadencia")]
        [Tooltip("Segundos entre disparos.")]
        public float fireInterval = 0.12f;
        [Tooltip("Balas por cargador. 0 = munición infinita.")]
        public int magazineSize = 30;
        public float reloadTime = 1.1f;

        [Header("Proyectil")]
        public Projectile projectilePrefab;
        public float projectileSpeed = 18f;

        [Header("Feedback")]
        public AudioClip shotSfx;
        public float recoil = 0.15f;
        public float screenShake = 0.06f;
    }
}
