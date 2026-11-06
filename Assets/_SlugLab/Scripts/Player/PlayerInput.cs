using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Lectura de teclado y ratón. Está aislada en su propio componente para que
    /// el resto del código no dependa de Unity: si mañana cambiamos a gamepad o
    /// a Input Actions, solo se toca este archivo.
    /// </summary>
    /// <remarks>
    /// Usamos el Input Manager clásico (<c>Input.GetAxisRaw</c>, <c>Input.GetKey</c>)
    /// porque funciona en cualquier laboratorio sin configurar paquetes.
    /// </remarks>
    [DefaultExecutionOrder(-100)]
    public class PlayerInput : MonoBehaviour
    {
        [Header("Teclas")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode fireKey = KeyCode.J;
        [SerializeField] private KeyCode meleeKey = KeyCode.K;
        [SerializeField] private KeyCode pauseKey = KeyCode.Escape;
        [SerializeField] private KeyCode reloadKey = KeyCode.R;

        [Header("Ratón")]
        [SerializeField] private bool useMouseButtons = true;

        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool FireHeld { get; private set; }
        public bool MeleePressed { get; private set; }
        public bool CrouchHeld { get; private set; }
        public bool PausePressed { get; private set; }
        public bool ReloadPressed { get; private set; }

        /// <summary>Bloquea la lectura (menús, muerte, cinemáticas).</summary>
        public bool IsLocked { get; set; }

        private void Update()
        {
            if (IsLocked)
            {
                Horizontal = 0f;
                Vertical = 0f;
                JumpPressed = false;
                JumpHeld = false;
                FireHeld = false;
                MeleePressed = false;
                CrouchHeld = false;
                PausePressed = false;
                ReloadPressed = false;
                return;
            }

            Horizontal = Input.GetAxisRaw("Horizontal");
            Vertical = Input.GetAxisRaw("Vertical");

            JumpPressed = Input.GetKeyDown(jumpKey);
            JumpHeld = Input.GetKey(jumpKey);
            CrouchHeld = Vertical < -0.5f || Input.GetKey(KeyCode.LeftControl);

            bool mouseFire = useMouseButtons && Input.GetMouseButton(0);
            bool mouseMelee = useMouseButtons && Input.GetMouseButtonDown(1);
            FireHeld = Input.GetKey(fireKey) || mouseFire;
            MeleePressed = Input.GetKeyDown(meleeKey) || mouseMelee;

            PausePressed = Input.GetKeyDown(pauseKey);
            ReloadPressed = Input.GetKeyDown(reloadKey);
        }
    }
}
