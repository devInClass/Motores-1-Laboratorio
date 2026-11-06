using SlugLab.Core;
using UnityEngine;

namespace SlugLab.Combat
{
    /// <summary>
    /// Pool de proyectiles de un mismo prefab. Evita instanciar/destruir decenas
    /// de balas por segundo: se crean una vez y se reciclan.
    /// </summary>
    /// <remarks>
    /// Las balas se guardan en un contenedor en la RAÍZ de la escena, nunca
    /// dentro del jugador. Si fueran hijas del jugador heredarían su movimiento
    /// y la velocidad de la bala cambiaría según hacia dónde corre: ese era el
    /// bug de "a veces avanza y a veces se queda quieta".
    /// </remarks>
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] private Projectile prefab;
        [SerializeField] private int prewarm = 12;

        private Pool<Projectile> _pool;
        private Transform _container;

        private Transform Container
        {
            get
            {
                if (_container == null)
                {
                    var go = new GameObject($"{name}_Balas");
                    _container = go.transform;
                }

                return _container;
            }
        }

        private void Awake()
        {
            if (prefab == null)
            {
                Debug.LogError($"[ProjectilePool] Falta el prefab en {name}.", this);
                enabled = false;
                return;
            }

            _pool = new Pool<Projectile>(Create, OnGet, OnRelease, prewarm);
        }

        private Projectile Create()
        {
            Projectile instance = Instantiate(prefab, Container);
            instance.SetPool(this);
            return instance;
        }

        private static void OnGet(Projectile projectile) => projectile.gameObject.SetActive(true);

        private void OnRelease(Projectile projectile)
        {
            if (projectile == null) return;
            projectile.gameObject.SetActive(false);
            projectile.transform.SetParent(Container, false);
        }

        public Projectile Get(Vector2 position, Quaternion rotation)
        {
            Projectile projectile = _pool.Get();
            projectile.transform.SetPositionAndRotation(position, rotation);
            return projectile;
        }

        public void Release(Projectile projectile) => _pool.Release(projectile);
    }
}
