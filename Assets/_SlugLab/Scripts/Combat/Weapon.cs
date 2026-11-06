using UnityEngine;

namespace SlugLab.Combat
{
    /// <summary>
    /// Lógica de un arma: cadencia, munición y recarga. Sin MonoBehaviour, para
    /// poder probarla con tiempos exactos en un test.
    /// </summary>
    public class Weapon
    {
        public WeaponDefinition Definition { get; }
        public int Ammo { get; private set; }
        public bool IsReloading { get; private set; }

        private float _nextFireAt;
        private float _reloadEndsAt;

        public Weapon(WeaponDefinition definition)
        {
            Definition = definition;
            Ammo = definition != null && definition.magazineSize > 0 ? definition.magazineSize : 0;
        }

        public bool HasInfiniteAmmo => Definition == null || Definition.magazineSize <= 0;

        public bool CanFire(float now)
            => Definition != null
               && now >= _nextFireAt
               && !IsReloading
               && (HasInfiniteAmmo || Ammo > 0);

        /// <summary>Dispara si es posible. Devuelve true y descuenta munición.</summary>
        public bool TryFire(float now)
        {
            if (!CanFire(now)) return false;

            _nextFireAt = now + Definition.fireInterval;

            if (!HasInfiniteAmmo)
            {
                Ammo--;
                if (Ammo <= 0) BeginReload(now);
            }

            return true;
        }

        public bool TryReload(float now)
        {
            if (Definition == null || HasInfiniteAmmo || IsReloading) return false;
            if (Ammo >= Definition.magazineSize) return false;
            BeginReload(now);
            return true;
        }

        /// <summary>Hay que llamarlo cada frame para terminar las recargas.</summary>
        public void Tick(float now)
        {
            if (!IsReloading || now < _reloadEndsAt) return;
            IsReloading = false;
            Ammo = Definition.magazineSize;
        }

        public float ReloadProgress(float now)
        {
            if (!IsReloading || Definition == null || Definition.reloadTime <= 0f) return 0f;
            float remaining = Mathf.Max(0f, _reloadEndsAt - now);
            return 1f - remaining / Definition.reloadTime;
        }

        /// <summary>Recarga instantánea (al recoger un arma o reaparecer).</summary>
        public void Refill()
        {
            IsReloading = false;
            _nextFireAt = 0f;
            Ammo = HasInfiniteAmmo ? 0 : Definition.magazineSize;
        }

        private void BeginReload(float now)
        {
            if (Definition == null) return;
            IsReloading = true;
            _reloadEndsAt = now + Definition.reloadTime;
        }
    }
}
