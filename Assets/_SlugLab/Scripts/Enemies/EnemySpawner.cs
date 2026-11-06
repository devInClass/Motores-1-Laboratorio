using System.Collections;
using SlugLab.Combat;
using SlugLab.Core;
using SlugLab.Level;
using UnityEngine;

namespace SlugLab.Enemies
{
    /// <summary>
    /// Genera una oleada de enemigos y abre la puerta cuando la zona queda limpia.
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Oleada")]
        [SerializeField] private GameObject[] enemies;
        [SerializeField] private float delayBetween = 0.35f;

        [Header("Puntos de aparición")]
        [SerializeField] private Transform[] spawnPoints;

        [Header("Puerta que se abre al limpiar")]
        [SerializeField] private LevelGate gate;

        [Header("Disparo")]
        [SerializeField] private bool autoStart = true;

        private int _alive;
        private bool _running;

        public bool IsFinished { get; private set; }
        public int AliveCount => _alive;

        private void Start()
        {
            if (autoStart) Begin();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!autoStart && other.CompareTag("Player")) Begin();
        }

        public void Begin()
        {
            if (_running || IsFinished) return;
            _running = true;
            StartCoroutine(RunWave());
        }

        private IEnumerator RunWave()
        {
            if (enemies != null && spawnPoints != null && spawnPoints.Length > 0)
            {
                foreach (GameObject prefab in enemies)
                {
                    if (prefab != null) Spawn(prefab);
                    yield return new WaitForSeconds(delayBetween);
                }

                while (_alive > 0) yield return null;
            }

            IsFinished = true;
            if (gate != null) gate.Open();
            GameSession.AddScore(250);
        }

        private void Spawn(GameObject prefab)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject instance = Instantiate(prefab, point.position, Quaternion.identity);

            Damageable damageable = instance.GetComponentInChildren<Damageable>();
            if (damageable == null) return;

            _alive++;
            damageable.Died += () => _alive--;
        }

        private void OnDrawGizmosSelected()
        {
            if (spawnPoints == null) return;
            Gizmos.color = Color.magenta;
            foreach (Transform point in spawnPoints)
            {
                if (point != null) Gizmos.DrawWireSphere(point.position, 0.4f);
            }
        }
    }
}
