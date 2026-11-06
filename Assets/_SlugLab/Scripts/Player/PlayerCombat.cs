using SlugLab.Combat;
using SlugLab.Level;
using SlugLab.Utils;
using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Disparo, apuntado en 8 direcciones y ataque cuerpo a cuerpo.
    /// </summary>
    /// <remarks>
    /// El apuntado alimenta dos cosas: la dirección real de la bala y los
    /// parámetros del blend tree (<c>AimAngle</c> y <c>Speed01</c>) que elige
    /// la pose de disparo.
    /// </remarks>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerInput input;
        [SerializeField] private PlayerController controller;
        [SerializeField] private PlayerAnimator animator;
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private Hitbox meleeHitbox;

        [Header("Arma inicial")]
        [SerializeField] private WeaponDefinition startingWeapon;

        [Header("Punto de salida")]
        [SerializeField] private float muzzleHeight = 0.8f;
        [SerializeField] private float muzzleDistance = 0.7f;

        [Header("Cuerpo a cuerpo")]
        [SerializeField] private float meleeActiveTime = 0.12f;
        [SerializeField] private float meleeCooldown = 0.4f;
        [SerializeField] private float meleeOffset = 0.75f;

        private Weapon _weapon;
        private Cooldown _meleeTimer;

        public Weapon CurrentWeapon => _weapon;

        /// <summary>¿Está apuntando? Dispara o mantiene ↑.</summary>
        public bool IsAiming => input != null && (input.FireHeld || input.Vertical > 0.5f);

        /// <summary>
        /// Dirección de apuntado en 8 sentidos: arriba, diagonales y horizontal.
        /// Hacia abajo solo en el aire (en el suelo, ↓ se usa para agacharse).
        /// </summary>
        public Vector2 AimDirection
        {
            get
            {
                if (input == null || controller == null) return Vector2.right;

                float horizontal = input.Horizontal;
                float vertical = input.Vertical;
                float facing = controller.Facing;

                if (vertical > 0.5f)
                {
                    float x = Mathf.Abs(horizontal) > 0.5f ? facing : 0f;
                    return new Vector2(x, 1f).normalized;
                }

                if (vertical < -0.5f && !controller.IsGrounded)
                {
                    float x = Mathf.Abs(horizontal) > 0.5f ? facing : 0f;
                    return new Vector2(x, -1f).normalized;
                }

                return new Vector2(facing, 0f);
            }
        }

        /// <summary>
        /// Ángulo de apuntado en grados para el blend tree: 90 = arriba,
        /// 45 = diagonal, 0 = horizontal, -45 = diagonal abajo, -90 = abajo.
        /// El volteo horizontal lo resuelve el Visual, por eso usamos |x|.
        /// </summary>
        public static float AimAngleDegrees(Vector2 direction)
        {
            if (direction.sqrMagnitude < 0.0001f) return 0f;
            return Mathf.Atan2(direction.y, Mathf.Abs(direction.x)) * Mathf.Rad2Deg;
        }

        private void Awake()
        {
            _weapon = new Weapon(startingWeapon);
            _meleeTimer = new Cooldown(meleeCooldown);
            if (meleeHitbox != null) meleeHitbox.Deactivate();
        }

        private void Update()
        {
            if (input == null || controller == null || !controller.IsAlive) return;

            _weapon.Tick(Time.time);

            if (input.ReloadPressed) _weapon.TryReload(Time.time);

            if (input.MeleePressed && _meleeTimer.TryConsume(Time.time))
            {
                PerformMelee();
            }

            if (input.FireHeld && _weapon.TryFire(Time.time))
            {
                Fire();
            }
        }

        private void Fire()
        {
            WeaponDefinition def = _weapon.Definition;
            if (def == null || def.projectilePrefab == null || projectilePool == null) return;

            Vector2 aim = AimDirection;
            Vector3 origin = transform.position + Vector3.up * muzzleHeight + (Vector3)(aim * muzzleDistance);

            for (int i = 0; i < def.projectilesPerShot; i++)
            {
                float spread = def.projectilesPerShot > 1
                    ? Mathf.Lerp(-def.spreadDegrees, def.spreadDegrees, (float)i / (def.projectilesPerShot - 1))
                    : 0f;

                Vector2 direction = (Vector2)(Quaternion.Euler(0f, 0f, spread) * aim);
                Projectile projectile = projectilePool.Get(origin, Quaternion.identity);
                projectile.Launch(direction, Team.Player, def.damage, def.projectileSpeed);
            }

            if (def.shotSfx != null) Audio.AudioManager.PlaySfx(def.shotSfx, 0.6f);
            if (def.screenShake > 0f) CameraFollow2D.ShakeCurrent(def.screenShake, 0.08f);
        }

        private void PerformMelee()
        {
            if (meleeHitbox == null) return;

            Vector3 local = meleeHitbox.transform.localPosition;
            local.x = Mathf.Abs(meleeOffset) * controller.Facing;
            meleeHitbox.transform.localPosition = local;

            meleeHitbox.Activate(meleeActiveTime);
            if (animator != null) animator.PlayMelee();
        }
    }
}
