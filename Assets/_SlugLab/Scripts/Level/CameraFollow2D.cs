using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Cámara 2D que sigue al jugador con suavizado, adelanta la vista hacia
    /// donde corre y se limita a los bordes del nivel. Incluye sacudida (shake).
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraFollow2D : MonoBehaviour
    {
        [Header("Objetivo")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 offset = new Vector2(0f, 1.2f);
        [SerializeField] private float smoothTime = 0.18f;
        [Tooltip("Cuánto se adelanta la cámara en la dirección del movimiento.")]
        [SerializeField] private float lookAhead = 1.4f;
        [SerializeField] private float lookAheadSpeed = 3f;

        [Header("Límites del nivel (centro de la cámara)")]
        [SerializeField] private bool useBounds = true;
        [SerializeField] private Vector2 minBounds = new Vector2(0f, 4f);
        [SerializeField] private Vector2 maxBounds = new Vector2(40f, 4f);

        [Header("Zoom")]
        [SerializeField] private float orthographicSize = 5.6f;

        public static CameraFollow2D Current { get; private set; }

        private Camera _camera;
        private Vector3 _velocity;
        private float _currentLookAhead;
        private float _shakeAmplitude;
        private float _shakeDuration;
        private float _shakeEndsAt = -1f;
        private Vector3 _basePosition;

        private void Awake()
        {
            Current = this;
            _camera = GetComponent<Camera>();
            _camera.orthographic = true;
            _camera.orthographicSize = orthographicSize;
        }

        private void Start()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
            }

            SnapToTarget();
        }

        private void OnDestroy()
        {
            if (Current == this) Current = null;
        }

        public void SetTarget(Transform newTarget) => target = newTarget;

        public void SetBounds(Vector2 min, Vector2 max)
        {
            useBounds = true;
            minBounds = min;
            maxBounds = max;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            float desiredLookAhead = 0f;
            Rigidbody2D rb = target.GetComponent<Rigidbody2D>();
            if (rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.5f)
            {
                desiredLookAhead = Mathf.Sign(rb.linearVelocity.x) * lookAhead;
            }

            _currentLookAhead = Mathf.MoveTowards(_currentLookAhead, desiredLookAhead, lookAheadSpeed * Time.deltaTime);

            Vector3 desired = new Vector3(
                target.position.x + offset.x + _currentLookAhead,
                target.position.y + offset.y,
                -10f);

            if (useBounds)
            {
                desired.x = Mathf.Clamp(desired.x, minBounds.x, maxBounds.x);
                desired.y = Mathf.Clamp(desired.y, minBounds.y, maxBounds.y);
            }

            _basePosition = Vector3.SmoothDamp(_basePosition, desired, ref _velocity, smoothTime);
            transform.position = _basePosition + ComputeShake();
        }

        public void SnapToTarget()
        {
            if (target == null) return;
            _basePosition = new Vector3(target.position.x + offset.x, target.position.y + offset.y, -10f);
            if (useBounds)
            {
                _basePosition.x = Mathf.Clamp(_basePosition.x, minBounds.x, maxBounds.x);
                _basePosition.y = Mathf.Clamp(_basePosition.y, minBounds.y, maxBounds.y);
            }

            transform.position = _basePosition;
        }

        public void Shake(float amplitude, float duration)
        {
            if (amplitude <= 0f || duration <= 0f) return;
            _shakeAmplitude = Mathf.Max(_shakeAmplitude, amplitude);
            _shakeDuration = Mathf.Max(_shakeDuration, duration);
            _shakeEndsAt = Time.time + _shakeDuration;
        }

        public static void ShakeCurrent(float amplitude, float duration)
        {
            if (Current != null) Current.Shake(amplitude, duration);
        }

        private Vector3 ComputeShake()
        {
            if (Time.time >= _shakeEndsAt || _shakeDuration <= 0f) return Vector3.zero;

            float remaining = Mathf.Clamp01((_shakeEndsAt - Time.time) / _shakeDuration);
            float amount = _shakeAmplitude * remaining;
            return (Vector3)(Random.insideUnitCircle * amount);
        }

        private void OnValidate()
        {
            if (_camera == null) _camera = GetComponent<Camera>();
            if (_camera != null) _camera.orthographicSize = orthographicSize;
        }
    }
}
