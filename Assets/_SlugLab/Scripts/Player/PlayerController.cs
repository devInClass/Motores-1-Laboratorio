using System;
using SlugLab.Combat;
using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Movimiento del jugador al estilo "run and gun" 2D: correr, saltar con
    /// coyote time y jump buffer, agacharse y recibir daño.
    /// </summary>
    /// <remarks>
    /// El movimiento se aplica en <c>FixedUpdate</c> (física) y la lectura de
    /// input en <c>Update</c> (frames). Mezclar ambos es el error nº1 de los
    /// proyectos de 2D: si mueves el Rigidbody en Update, la física se comporta
    /// distinto según los FPS.
    /// </remarks>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Damageable))]
    public class PlayerController : MonoBehaviour
    {
        public enum State
        {
            Idle,
            Run,
            Jump,
            Fall,
            Crouch,
            Hurt,
            Dead
        }

        [Header("Referencias")]
        [SerializeField] private PlayerInput input;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.62f, 0.14f);
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private BoxCollider2D bodyCollider;

        [Header("Movimiento horizontal")]
        [SerializeField] private float runSpeed = 6.5f;
        [SerializeField] private float acceleration = 60f;
        [SerializeField] private float deceleration = 85f;
        [Tooltip("0 = sin control en el aire, 1 = control total.")]
        [SerializeField] private float airControl = 0.6f;
        [SerializeField] private float crouchSpeedMultiplier = 0.35f;

        [Header("Salto")]
        [SerializeField] private float jumpVelocity = 13.5f;
        [Tooltip("Gravedad extra al caer: hace el salto menos flotante.")]
        [SerializeField] private float fallGravityMultiplier = 1.9f;
        [Tooltip("Gravedad extra al soltar el salto antes de tiempo (salto variable).")]
        [SerializeField] private float jumpCutMultiplier = 2.4f;
        [Tooltip("Margen para saltar justo después de salir de una plataforma.")]
        [SerializeField] private float coyoteTime = 0.1f;
        [Tooltip("Margen para registrar el salto antes de tocar el suelo.")]
        [SerializeField] private float jumpBufferTime = 0.12f;
        [SerializeField] private int extraAirJumps = 0;

        [Header("Daño")]
        [SerializeField] private float hurtLockTime = 0.3f;
        [SerializeField] private float deathDelay = 1.2f;

        public State CurrentState { get; private set; } = State.Idle;
        public bool IsGrounded { get; private set; }
        public bool IsCrouching { get; private set; }
        public int Facing { get; private set; } = 1;
        public float HorizontalInput { get; private set; }
        public Vector2 Velocity => _rb != null ? _rb.linearVelocity : Vector2.zero;

        /// <summary>Velocidad horizontal normalizada 0–1 (la usa el blend tree).</summary>
        public float SpeedNormalized => _rb == null ? 0f : Mathf.Clamp01(Mathf.Abs(_rb.linearVelocity.x) / Mathf.Max(0.01f, runSpeed));
        public bool IsAlive => _damageable != null && _damageable.IsAlive;
        public Damageable Damageable => _damageable;

        public event Action<State> StateChanged;
        public event Action Died;

        private Rigidbody2D _rb;
        private Damageable _damageable;
        private Vector2 _standSize;
        private Vector2 _standOffset;
        private Vector2 _crouchSize;
        private Vector2 _crouchOffset;
        private float _coyoteUntil;
        private float _jumpBufferUntil;
        private float _hurtUntil;
        private float _deathAt = -1f;
        private int _airJumpsLeft;
        private bool _controlEnabled = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _damageable = GetComponent<Damageable>();

            if (bodyCollider != null)
            {
                _standSize = bodyCollider.size;
                _standOffset = bodyCollider.offset;
                _crouchSize = new Vector2(_standSize.x, _standSize.y * 0.55f);
                _crouchOffset = new Vector2(_standOffset.x, _standOffset.y - _standSize.y * 0.225f);
            }

            _damageable.Damaged += OnDamaged;
            _damageable.Died += OnDied;
        }

        private void OnDestroy()
        {
            if (_damageable == null) return;
            _damageable.Damaged -= OnDamaged;
            _damageable.Died -= OnDied;
        }

        private void Update()
        {
            if (input == null) return;

            HorizontalInput = _controlEnabled ? input.Horizontal : 0f;

            if (_controlEnabled)
            {
                if (input.JumpPressed) _jumpBufferUntil = Time.time + jumpBufferTime;
                UpdateCrouch(input.CrouchHeld);
                if (Mathf.Abs(HorizontalInput) > 0.1f) Facing = HorizontalInput > 0f ? 1 : -1;
            }

            UpdateState();

            if (_deathAt > 0f && Time.time >= _deathAt)
            {
                _deathAt = -1f;
                Died?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            if (!_controlEnabled || _damageable.IsDead) return;

            UpdateGrounded();

            if (IsGrounded)
            {
                _coyoteUntil = Time.time + coyoteTime;
                _airJumpsLeft = extraAirJumps;
            }

            TryJump();
            ApplyHorizontalMovement();
            ApplyExtraGravity();
        }

        private void UpdateGrounded()
        {
            if (groundCheck == null)
            {
                IsGrounded = false;
                return;
            }

            Collider2D hit = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundMask);
            IsGrounded = hit != null && hit.gameObject != gameObject;
        }

        private void TryJump()
        {
            bool bufferActive = Time.time <= _jumpBufferUntil;
            if (!bufferActive || input == null) return;

            bool canGroundJump = IsGrounded || Time.time <= _coyoteUntil;
            bool canAirJump = !canGroundJump && _airJumpsLeft > 0;

            if (!canGroundJump && !canAirJump) return;

            if (canAirJump) _airJumpsLeft--;

            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpVelocity);
            _coyoteUntil = 0f;
            _jumpBufferUntil = 0f;
            IsGrounded = false;
        }

        private void ApplyHorizontalMovement()
        {
            Vector2 v = _rb.linearVelocity;
            float maxSpeed = IsCrouching ? runSpeed * crouchSpeedMultiplier : runSpeed;
            float targetSpeed = HorizontalInput * maxSpeed;

            float rate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
            if (!IsGrounded) rate *= airControl;

            v.x = Mathf.MoveTowards(v.x, targetSpeed, rate * Time.fixedDeltaTime);
            _rb.linearVelocity = v;
        }

        private void ApplyExtraGravity()
        {
            Vector2 v = _rb.linearVelocity;
            if (v.y < -0.01f)
            {
                v.y += Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
            }
            else if (v.y > 0.01f && input != null && !input.JumpHeld)
            {
                v.y += Physics2D.gravity.y * (jumpCutMultiplier - 1f) * Time.fixedDeltaTime;
            }

            _rb.linearVelocity = v;
        }

        private void UpdateCrouch(bool wantsCrouch)
        {
            if (!wantsCrouch || !IsGrounded)
            {
                if (IsCrouching && CanStandUp()) SetCrouch(false);
                return;
            }

            SetCrouch(true);
        }

        private void SetCrouch(bool crouch)
        {
            if (IsCrouching == crouch) return;
            IsCrouching = crouch;

            if (bodyCollider == null) return;
            bodyCollider.size = crouch ? _crouchSize : _standSize;
            bodyCollider.offset = crouch ? _crouchOffset : _standOffset;
        }

        /// <summary>Evita levantarse si hay techo encima.</summary>
        private bool CanStandUp()
        {
            if (bodyCollider == null) return true;
            Vector2 center = (Vector2)transform.position + _standOffset;
            return Physics2D.OverlapBox(center, _standSize * 0.95f, 0f, groundMask) == null;
        }

        private void UpdateState()
        {
            State next;

            if (_damageable != null && _damageable.IsDead)
            {
                next = State.Dead;
            }
            else if (Time.time < _hurtUntil)
            {
                next = State.Hurt;
            }
            else if (IsCrouching)
            {
                next = State.Crouch;
            }
            else if (!IsGrounded)
            {
                next = _rb.linearVelocity.y > 0.1f ? State.Jump : State.Fall;
            }
            else
            {
                next = Mathf.Abs(_rb.linearVelocity.x) > 0.2f ? State.Run : State.Idle;
            }

            SetState(next);
        }

        private void SetState(State next)
        {
            if (next == CurrentState) return;
            CurrentState = next;
            StateChanged?.Invoke(next);
        }

        private void OnDamaged(DamageInfo info)
        {
            _hurtUntil = Time.time + hurtLockTime;
            SetState(State.Hurt);
        }

        private void OnDied()
        {
            _controlEnabled = false;
            IsCrouching = false;
            if (bodyCollider != null)
            {
                bodyCollider.size = _standSize;
                bodyCollider.offset = _standOffset;
            }

            _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
            SetState(State.Dead);
            _deathAt = Time.time + deathDelay;
        }

        /// <summary>Coloca al jugador en un punto y lo cura (checkpoint / reinicio).</summary>
        public void RespawnAt(Vector2 position)
        {
            transform.position = position;
            _rb.linearVelocity = Vector2.zero;
            _damageable.Revive();
            _controlEnabled = true;
            _hurtUntil = 0f;
            _deathAt = -1f;
            IsCrouching = false;
            SetState(State.Idle);
        }

        public void SetControlEnabled(bool enabled)
        {
            _controlEnabled = enabled;
            if (!enabled) HorizontalInput = 0f;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
        }
#endif
    }
}
