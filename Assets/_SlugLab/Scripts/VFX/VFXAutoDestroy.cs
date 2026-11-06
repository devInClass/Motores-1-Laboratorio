using UnityEngine;

namespace SlugLab.VFX
{
    /// <summary>
    /// Destruye el objeto después de unos segundos. Se usa en prefabs de VFX
    /// para que una explosión no se quede viva para siempre en la escena.
    /// </summary>
    public class VFXAutoDestroy : MonoBehaviour
    {
        [SerializeField] private float delay = 2f;

        private void Start()
        {
            Destroy(gameObject, delay);
        }
    }
}
