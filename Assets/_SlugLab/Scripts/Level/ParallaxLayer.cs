using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Capa de fondo con parallax: se mueve a una fracción de la velocidad de la
    /// cámara. <c>factor = 0</c> la deja fija (cielo), <c>factor = 1</c> la pega
    /// al mundo (no se usa para fondos).
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] private float factor = 0.4f;
        [SerializeField, Range(0f, 1f)] private float verticalFactor = 0.15f;

        private Vector3 _startPosition;
        private Vector3 _cameraStart;

        private void Start()
        {
            _startPosition = transform.position;

            Camera camera = Camera.main;
            if (camera != null) _cameraStart = camera.transform.position;
        }

        private void LateUpdate()
        {
            Camera camera = Camera.main;
            if (camera == null) return;

            Vector3 delta = camera.transform.position - _cameraStart;
            transform.position = new Vector3(
                _startPosition.x + delta.x * factor,
                _startPosition.y + delta.y * verticalFactor,
                _startPosition.z);
        }
    }
}
