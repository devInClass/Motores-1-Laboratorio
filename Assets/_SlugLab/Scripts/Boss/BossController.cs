using System;
using System.Collections;
using SlugLab.Combat;
using SlugLab.Core;
using SlugLab.Level;
using UnityEngine;

namespace SlugLab.Boss
{
    /// <summary>
    /// Jefe final con tres fases. Cada fase cambia la agresividad: más ráfagas,
    /// embestidas y refuerzos. Se derrota al bajarle toda la vida.
    /// </summary>
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossController : MonoBehaviour
    {
        [Serializable]
        public class BossPhase
        {
            [Range(0f, 1f)] public float healthThreshold = 1f;
            public float actionInterval = 2.2f;
            public int volleyCount = 5;
            public float volleySpread = 50f;
            public bool canCharge;
            public bool canSummon;
        }

        [Header("Identidad")]
        [SerializeField] private string displayName = "CAPITAN MUELAS";
        [SerializeField] private int maxHealth = 60;

        [Header("Fases (de mayor a menor vida)")]
        [SerializeField] private BossPhase[] phases =
        {
            new BossPhase { healthThreshold = 1f, actionInterval = 2.4f, volleyCount = 3, volleySpread = 25f },
            new BossPhase { healthThreshold = 0.66f, actionInterval = 1.9f, volleyCount = 5, volleySpread = 40f, canCharge = true },
            new BossPhase { healthThreshold = 0.33f, actionInterval = 1.4f, volleyCount = 7, volleySpread = 55f, canCharge = true, canSummon = true }
        };

        [Header("Ataques")]
        [SerializeField] private ProjectilePool projectilePool;
        [SerializeField] private Transform muzzle;
        [SerializeField] private Transform summonPoint;
        [SerializeField] private GameObject minionPrefab;
        [SerializeField] private int volleyDamage = 1;
        [SerializeField] private float volleySpeed = 9f;
        [SerializeField] private float chargeSpeed = 11f;
        [SerializeField] private float chargeDuration = 0.65f;
        [SerializeField] private LayerMask groundMask;

        [Header("Ritmo")]
        [SerializeField] private float introDelay = 1.2f;
        [SerializeField] private float phaseTransitionTime = 0.9f;

        public string DisplayName => displayName;
        public int CurrentPhaseIndex { get; private set; }
        public float HealthNormalized => _damageable != null ? _damageable.Health.Normalized : 0f;
        public bool IsAlive => _damageable != null && _damageable.IsAlive;

        public event Action<float> HealthChanged;
        public event Action<int> PhaseChanged;

        private Damageable _damageable;
        private Rigidbody2D _rb;
        private Animator _animator;
        private Transform _player;
        private bool _dead;
        private bool _transitioning;
        private int _facing = -1;

        private static readonly int VolleyHash = Animator.StringToHash("Volley");
        private static readonly int ChargeHash = Animator.StringToHash("Charge");
        private static readonly int SummonHash = Animator.StringToHash("Summon");
        private static readonly int DeadHash = Animator.StringToHash("Dead");

        private BossPhase CurrentPhase
        {
            get
            {
                if (phases == null || phases.Length == 0)
                    return new BossPhase();

                return phases[Mathf.Clamp(CurrentPhaseIndex, 0, phases.Length - 1)];
            }
        }

        private void Awake()
        {
            _damageable = GetComponent<Damageable>();
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            _damageable.Initialize(maxHealth);
            _damageable.Died += OnDied;
            _damageable.Health.Changed += OnHealthChanged;

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) _player = playerObject.transform;

            HealthChanged?.Invoke(HealthNormalized);
            PhaseChanged?.Invoke(0);

            StartCoroutine(BehaviourLoop());
        }

        private void OnDestroy()
        {
            if (_damageable == null) return;
            _damageable.Died -= OnDied;
            if (_damageable.Health != null) _damageable.Health.Changed -= OnHealthChanged;
        }

        private void Update()
        {
            if (_dead) return;

            if (phases != null && CurrentPhaseIndex + 1 < phases.Length)
            {
                if (HealthNormalized <= phases[CurrentPhaseIndex + 1].healthThreshold)
                {
                    CurrentPhaseIndex++;
                    PhaseChanged?.Invoke(CurrentPhaseIndex);
                    StartCoroutine(PhaseTransition());
                }
            }
        }

        private void OnHealthChanged(int current, int max)
        {
            HealthChanged?.Invoke(HealthNormalized);
        }

        private IEnumerator PhaseTransition()
        {
            _transitioning = true;
            _damageable.IsInvulnerable = true;
            _rb.linearVelocity = Vector2.zero;

            if (_animator != null) _animator.SetTrigger("Phase");
            CameraFollow2D.ShakeCurrent(0.28f, 0.35f);

            yield return new WaitForSeconds(phaseTransitionTime);

            _damageable.IsInvulnerable = false;
            _transitioning = false;
        }

        private IEnumerator BehaviourLoop()
        {
            yield return new WaitForSeconds(introDelay);

            while (IsAlive && !_dead)
            {
                while (_transitioning) yield return null;

                yield return StartCoroutine(PerformAction());

                if (!IsAlive) yield break;
                yield return new WaitForSeconds(CurrentPhase.actionInterval);
            }
        }

        private IEnumerator PerformAction()
        {
            BossPhase phase = CurrentPhase;
            float roll = UnityEngine.Random.value;

            if (phase.canCharge && roll < 0.3f)
            {
                yield return StartCoroutine(Charge());
            }
            else if (phase.canSummon && roll < 0.55f)
            {
                Summon();
                yield return new WaitForSeconds(0.4f);
            }
            else
            {
                Volley(phase);
                yield return new WaitForSeconds(0.3f);
            }
        }

        private void Volley(BossPhase phase)
        {
            if (projectilePool == null) return;
            if (_animator != null) _animator.SetTrigger(VolleyHash);

            Vector2 origin = muzzle != null ? muzzle.position : transform.position;
            Vector2 target = _player != null ? (Vector2)_player.position + Vector2.up * 0.6f : origin + Vector2.right;
            Vector2 baseDirection = (target - origin).normalized;

            int count = Mathf.Max(1, phase.volleyCount);
            float halfSpread = phase.volleySpread * 0.5f;

            for (int i = 0; i < count; i++)
            {
                float t = count == 1 ? 0.5f : (float)i / (count - 1);
                float angle = Mathf.Lerp(-halfSpread, halfSpread, t);
                Vector2 direction = (Vector2)(Quaternion.Euler(0f, 0f, angle) * baseDirection);

                Projectile projectile = projectilePool.Get(origin, Quaternion.identity);
                projectile.Launch(direction, Team.Enemy, volleyDamage, volleySpeed);
            }
        }

        private IEnumerator Charge()
        {
            if (_animator != null) _animator.SetTrigger(ChargeHash);
            if (_player != null)
            {
                int desired = _player.position.x >= transform.position.x ? 1 : -1;
                if (desired != _facing) Turn(desired);
            }

            float endTime = Time.time + chargeDuration;
            while (Time.time < endTime && IsAlive)
            {
                _rb.linearVelocity = new Vector2(_facing * chargeSpeed, _rb.linearVelocity.y);

                Vector2 wallOrigin = (Vector2)transform.position + Vector2.up * 0.5f;
                if (Physics2D.Raycast(wallOrigin, Vector2.right * _facing, 0.9f, groundMask).collider != null) break;

                yield return null;
            }

            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            CameraFollow2D.ShakeCurrent(0.12f, 0.2f);
        }

        private void Summon()
        {
            if (minionPrefab == null) return;
            if (_animator != null) _animator.SetTrigger(SummonHash);

            Vector3 origin = summonPoint != null ? summonPoint.position : transform.position + Vector3.right * _facing * 2.5f;

            Instantiate(minionPrefab, origin + Vector3.up * 0.5f, Quaternion.identity);
            Instantiate(minionPrefab, origin - Vector3.right * 2.5f + Vector3.up * 0.5f, Quaternion.identity);
        }

        private void Turn(int direction)
        {
            _facing = direction;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }

        private void OnDied()
        {
            if (_dead) return;
            _dead = true;

            StopAllCoroutines();
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false;

            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>()) collider.enabled = false;

            if (_animator != null) _animator.SetTrigger(DeadHash);
            CameraFollow2D.ShakeCurrent(0.5f, 0.8f);
            GameSession.MarkBossDefeated();

            StartCoroutine(VictoryRoutine());
        }

        private IEnumerator VictoryRoutine()
        {
            yield return new WaitForSeconds(1.4f);
            if (GameFlow.Instance != null) GameFlow.Instance.Win();
        }
    }
}
