using UnityEngine;

namespace SlugLab.Level
{
    /// <summary>
    /// Plataforma que va y viene entre dos puntos. El jugador que la pisa se
    /// convierte en hijo temporal para que no se resbale.
    /// </summary>
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] private Vector3 pointA;
        [SerializeField] private Vector3 pointB = new Vector3(4f, 0f, 0f);
        [SerializeField] private float speed = 2.5f;
        [SerializeField] private float waitAtEnds = 0.6f;

        private Vector3 _target;
        private float _waitUntil;

        private void Start()
        {
            pointA = transform.position;
            pointB = pointA + pointB;
            _target = pointB;
        }

        private void FixedUpdate()
        {
            if (Time.time < _waitUntil) return;

            transform.position = Vector3.MoveTowards(transform.position, _target, speed * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, _target) < 0.01f)
            {
                _target = _target == pointA ? pointB : pointA;
                _waitUntil = Time.time + waitAtEnds;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player")) collision.transform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player")) collision.transform.SetParent(null);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, transform.position + pointB);
        }
    }
}
