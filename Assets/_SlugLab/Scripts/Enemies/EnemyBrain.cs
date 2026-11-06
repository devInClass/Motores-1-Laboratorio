using SlugLab.Combat;
using SlugLab.Core;
using SlugLab.Utils;
using UnityEngine;

namespace SlugLab.Enemies
{
    /// <summary>
    /// Cerebro de un enemigo terrestre: patrulla, detecta al jugador, lo persigue
    /// y dispara. Es una máquina de estados finita (FSM) sencilla.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Damageable))]
    public class EnemyBrain : MonoBehaviour
    {
        public enum State
        {
            Patrol,
            Alert,
            Chase,
            Attack,
            Stagger,
            Dead
        }

        [Header("Configuración")]
        [SerializeField] private EnemyConfig config;

        [Header("Detección")]
        [SerializeField] private Transform eye;
        [SerializeField] private Transform groundProbe;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private LayerMask playerMask;

        [Header("Referencias")]
        [SerializeField] private EnemyGun gun;
        [SerializeField] private Animator animator;

        public State CurrentState { get; private set; } = State.Patrol;

        private Rigidbody2D _rb;
        private Damageable _damageable;
        private SpriteRenderer _renderer;
        private Transform _player;
        private int _facing = 1;
        private float _patrolMinX;
        private float _patrolMaxX;
        private float _alertUntil;
        private float _staggerUntil;
        private readonly Cooldown _attackCooldown = new Cooldown(1.5f);
        private float _moveDirection;
        private bool _dead;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int HurtHash = Animator.StringToHash("Hurt");
        private static readonly int DeadHash = Animator.StringToHash("Dead");

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _damageable = GetComponent<Damageable>();
            _renderer = GetComponentInChildren<SpriteRenderer>();

            _damageable.Damaged += OnDamaged;
            _damageable.Died += OnDied;
        }

        private void Start()
        {
            if (config != null)
            {
                _damageable.Initialize(config.maxHealth);
                _attackCooldown.Duration = config.attackInterval;
                _patrolMinX = transform.position.x - config.patrolDistance;
                _patrolMaxX = transform.position.x + config.patrolDistance;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) _player = playerObject.transform;
        }

        private void OnDestroy()
        {
            if (_damageable == null) return;
            _damageable.Damaged -= OnDamaged;
            _damageable.Died -= OnDied;
        }

        private void Update()
        {
            if (_dead) return;

            if (_player == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null) _player = playerObject.transform;
            }

            float distance = _player != null ? Vector2.Distance(transform.position, _player.position) : float.MaxValue;
            bool seesPlayer = _player != null && distance <= config.sightRange && HasLineOfSight();

            switch (CurrentState)
            {
                case State.Patrol:
                    Patrol();
                    if (seesPlayer) EnterAlert();
                    break;

                case State.Alert:
                    _moveDirection = 0f;
                    FaceTarget(_player);
                    if (Time.time >= _alertUntil) SetState(State.Chase);
                    break;

                case State.Chase:
                    Chase(distance, seesPlayer);
                    break;

                case State.Attack:
                    _moveDirection = 0f;
                    FaceTarget(_player);
                    if (Time.time >= _staggerUntil && _attackCooldown.IsReady(Time.time))
                    {
                        SetState(State.Chase);
                    }
                    break;

                case State.Stagger:
                    _moveDirection = 0f;
                    if (Time.time >= _staggerUntil) SetState(State.Chase);
                    break;
            }

            UpdateAnimation();
        }

        private void FixedUpdate()
        {
            if (_dead || config == null) return;

            float speed = CurrentState == State.Patrol ? config.patrolSpeed : config.chaseSpeed;
            _rb.linearVelocity = new Vector2(_moveDirection * speed, _rb.linearVelocity.y);
        }

        private void Patrol()
        {
            bool atEdge = IsAtEdge();
            bool atWall = IsFacingWall();

            if (transform.position.x <= _patrolMinX && _facing < 0) Turn();
            else if (transform.position.x >= _patrolMaxX && _facing > 0) Turn();
            else if (atEdge || atWall) Turn();

            _moveDirection = _facing;
        }

        private void Chase(float distance, bool seesPlayer)
        {
            if (distance > config.loseSightRange)
            {
                SetState(State.Patrol);
                return;
            }

            FaceTarget(_player);

            if (distance <= config.attackRange && _attackCooldown.IsReady(Time.time))
            {
                Attack();
                return;
            }

            _moveDirection = Mathf.Abs(distance) > config.attackRange * 0.8f ? _facing : 0f;

            if (!seesPlayer) _moveDirection = 0f;
        }

        private void Attack()
        {
            SetState(State.Attack);
            _staggerUntil = Time.time + 0.25f;

            if (animator != null) animator.SetTrigger(AttackHash);

            if (gun != null && _player != null)
            {
                Vector2 direction = ((Vector2)_player.position + Vector2.up * 0.8f - (Vector2)gun.MuzzlePosition).normalized;
                gun.Fire(direction);
            }

            _attackCooldown.TryConsume(Time.time);
        }

        private void EnterAlert()
        {
            SetState(State.Alert);
            _alertUntil = Time.time + config.alertTime;
            FaceTarget(_player);
        }

        private void SetState(State next)
        {
            if (CurrentState == next) return;
            CurrentState = next;
            if (next != State.Chase && next != State.Patrol) _moveDirection = 0f;
        }

        private void Turn()
        {
            _facing *= -1;
            if (_renderer != null)
            {
                Vector3 scale = _renderer.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * _facing;
                _renderer.transform.localScale = scale;
            }
        }

        private void FaceTarget(Transform target)
        {
            if (target == null) return;
            int desired = target.position.x >= transform.position.x ? 1 : -1;
            if (desired != _facing) Turn();
        }

        private bool HasLineOfSight()
        {
            if (_player == null) return false;
            Vector2 origin = eye != null ? eye.position : (Vector2)transform.position + Vector2.up * 0.6f;
            Vector2 target = (Vector2)_player.position + Vector2.up * 0.6f;
            RaycastHit2D hit = Physics2D.Raycast(origin, (target - origin).normalized, config.sightRange, groundMask);
            return hit.collider == null;
        }

        private bool IsAtEdge()
        {
            if (config == null) return false;
            Vector2 origin = groundProbe != null
                ? groundProbe.position
                : (Vector2)transform.position + new Vector2(_facing * 0.55f, 0f);

            return Physics2D.Raycast(origin, Vector2.down, 1.2f, groundMask).collider == null;
        }

        private bool IsFacingWall()
        {
            Vector2 origin = (Vector2)transform.position + Vector2.up * 0.5f;
            return Physics2D.Raycast(origin, Vector2.right * _facing, 0.55f, groundMask).collider != null;
        }

        private void UpdateAnimation()
        {
            if (animator == null) return;
            animator.SetFloat(SpeedHash, Mathf.Abs(_rb.linearVelocity.x));
        }

        private void OnDamaged(DamageInfo info)
        {
            if (_dead) return;
            _staggerUntil = Time.time + 0.22f;
            SetState(State.Stagger);
            if (animator != null) animator.SetTrigger(HurtHash);
        }

        private void OnDied()
        {
            if (_dead) return;
            _dead = true;
            SetState(State.Dead);
            _moveDirection = 0f;

            if (config != null) GameSession.AddScore(config.scoreValue);
            if (animator != null) animator.SetTrigger(DeadHash);

            foreach (Collider2D collider in GetComponentsInChildren<Collider2D>()) collider.enabled = false;
            _rb.linearVelocity = Vector2.zero;
            _rb.simulated = false;

            Destroy(gameObject, config != null ? config.deathDelay : 0.6f);
        }

        private void OnDrawGizmosSelected()
        {
            if (config == null) return;
            Gizmos.color = new Color(1f, 0.6f, 0f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, config.sightRange);
            Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
            Gizmos.DrawWireSphere(transform.position, config.attackRange);
        }
    }
}
