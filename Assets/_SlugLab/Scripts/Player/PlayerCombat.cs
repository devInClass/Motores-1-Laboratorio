using SlugLab.Combat;
using SlugLab.Level;
using SlugLab.Utils;
using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Disparo, apuntado y ataque cuerpo a cuerpo del jugador.
    /// </summary>
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerInput input;
        [SerializeField] private PlayerController controller;
        [SerializeField] private PlayerAnimator animator;
        [SerializeField] private Transform muzzle;
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private Hitbox meleeHitbox;

        [Header("Arma inicial")]
        [SerializeField] private WeaponDefinition startingWeapon;

        [Header("Cuerpo a cuerpo")]
        [SerializeField] private float meleeActiveTime = 0.12f;
        [SerializeField] private float meleeCooldown = 0.4f;
        [SerializeField] private float meleeOffset = 0.75f;

        private Weapon _weapon;
        private Cooldown _meleeTimer;

        public Weapon CurrentWeapon => _weapon;

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

        /// <summary>Hacia dónde apunta: arriba si se pulsa ↑, si no hacia donde mira.</summary>
        public Vector2 AimDirection
        {
            get
            {
                if (input != null && input.Vertical > 0.5f) return Vector2.up;
                return controller != null && controller.Facing < 0 ? Vector2.left : Vector2.right;
            }
        }

        private void Fire()
        {
            WeaponDefinition def = _weapon.Definition;
            if (def == null || def.projectilePrefab == null || projectilePool == null) return;

            Vector2 aim = AimDirection;

            for (int i = 0; i < def.projectilesPerShot; i++)
            {
                float spread = def.projectilesPerShot > 1
                    ? Mathf.Lerp(-def.spreadDegrees, def.spreadDegrees, (float)i / (def.projectilesPerShot - 1))
                    : 0f;

                Vector2 direction = (Vector2)(Quaternion.Euler(0f, 0f, spread) * aim);
                Vector3 spawnPosition = muzzle != null ? muzzle.position : transform.position;
                Projectile projectile = projectilePool.Get(spawnPosition, Quaternion.identity);
                projectile.Launch(direction, Team.Player, def.damage, def.projectileSpeed);
            }

            if (muzzle != null)
            {
                Vector3 local = muzzle.localPosition;
                local.x = Mathf.Abs(local.x) * controller.Facing;
                muzzle.localPosition = local;
            }

            if (animator != null) animator.PlayShoot();
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
