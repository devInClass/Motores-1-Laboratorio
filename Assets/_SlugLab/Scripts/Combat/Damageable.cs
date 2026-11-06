using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SlugLab.Combat
{
    /// <summary>
    /// Puente entre el mundo de Unity (colliders, frames) y la clase pura
    /// <see cref="Combat.Health"/>. Añade invulnerabilidad temporal, empuje y
    /// parpadeo para que el jugador "sienta" el golpe.
    /// </summary>
    [DisallowMultipleComponent]
    public class Damageable : MonoBehaviour, IDamageable
    {
        [Header("Vida")]
        [SerializeField] private int maxHealth = 3;

        [Header("Reacción al daño")]
        [Tooltip("Segundos de invulnerabilidad después de recibir un golpe.")]
        [SerializeField] private float invulnerabilityTime = 0.7f;
        [SerializeField] private float knockbackForce = 7f;
        [SerializeField] private float flashDuration = 0.12f;

        [Header("Bando")]
        [SerializeField] private Team team = Team.Enemy;

        [Header("Eventos")]
        [SerializeField] private UnityEvent onDamaged;
        [SerializeField] private UnityEvent onDied;

        public Health Health { get; private set; }
        public Team Team => team;
        public bool IsAlive => Health != null && !Health.IsDead;
        public bool IsDead => Health != null && Health.IsDead;
        public bool IsInvulnerable { get; set; }

        /// <summary>Se dispara solo cuando el daño se aplicó de verdad.</summary>
        public event Action<DamageInfo> Damaged;
        public event Action Died;

        private Rigidbody2D _rb;
        private SpriteRenderer _renderer;
        private Color _baseColor;
        private float _invulnerableUntil;
        private Coroutine _flashRoutine;

        private void Awake()
        {
            Health = new Health(maxHealth);
            _rb = GetComponent<Rigidbody2D>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
            if (_renderer != null) _baseColor = _renderer.color;

            Health.Died += HandleDeath;
        }

        /// <summary>
        /// Reconfigura la vida máxima. Se usa cuando un enemigo lee su vida de un
        /// <c>EnemyConfig</c> y no del prefab.
        /// </summary>
        public void Initialize(int max)
        {
            maxHealth = Mathf.Max(1, max);

            if (Health != null) Health.Died -= HandleDeath;
            Health = new Health(maxHealth);
            Health.Died += HandleDeath;

            _invulnerableUntil = 0f;
            IsInvulnerable = false;
        }

        private void OnDestroy()
        {
            if (Health != null) Health.Died -= HandleDeath;
        }

        public bool ApplyDamage(DamageInfo info)
        {
            if (Health == null || Health.IsDead) return false;
            if (IsInvulnerable || Time.time < _invulnerableUntil) return false;
            if (!Health.ApplyDamage(info)) return false;

            _invulnerableUntil = Time.time + invulnerabilityTime;
            ApplyKnockback(info);
            PlayFlash();

            onDamaged?.Invoke();
            Damaged?.Invoke(info);
            return true;
        }

        public void Heal(int amount)
        {
            if (!IsAlive) return;
            Health.Heal(amount);
        }

        public void HealToFull()
        {
            if (Health == null) return;
            Health.ResetToFull();
        }

        /// <summary>Revive a la entidad al máximo (se usa al reaparecer en un checkpoint).</summary>
        public void Revive()
        {
            if (Health == null) return;
            Health.ResetToFull();
            _invulnerableUntil = Time.time + 1.2f;
            gameObject.SetActive(true);
        }

        private void ApplyKnockback(DamageInfo info)
        {
            if (_rb == null || info.Knockback <= 0f) return;
            _rb.linearVelocity = new Vector2(info.Direction.x * info.Knockback, Mathf.Max(_rb.linearVelocity.y, info.Knockback * 0.35f));
        }

        private void PlayFlash()
        {
            if (_renderer == null) return;
            if (_flashRoutine != null) StopCoroutine(_flashRoutine);
            _flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            _renderer.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
            _renderer.color = _baseColor;
            _flashRoutine = null;
        }

        private void HandleDeath()
        {
            Died?.Invoke();
            onDied?.Invoke();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (maxHealth < 1) maxHealth = 1;
        }
#endif
    }
}
