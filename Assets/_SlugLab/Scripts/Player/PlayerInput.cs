using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Lectura de teclado, ratón y controles táctiles. Es la única puerta de
    /// entrada del jugador: si mañana cambiamos a gamepad, solo se toca este
    /// archivo.
    /// </summary>
    /// <remarks>
    /// Usamos el Input Manager clásico (<c>Input.GetAxisRaw</c>, <c>Input.GetKey</c>)
    /// porque funciona en cualquier laboratorio sin configurar paquetes.
    /// En móvil se suman las órdenes de <see cref="VirtualInput"/> y se apaga el
    /// ratón para que tocar la pantalla no dispare sin querer.
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

        [Header("Entrada táctil")]
        [SerializeField] private bool useVirtualInput = true;

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

        private void Awake()
        {
            if (Application.isMobilePlatform) useMouseButtons = false;
        }

        private void Update()
        {
            if (IsLocked)
            {
                Clear();
                return;
            }

            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            if (useVirtualInput)
            {
                horizontal = Mathf.Clamp(horizontal + VirtualInput.Horizontal, -1f, 1f);
                vertical = Mathf.Clamp(vertical + VirtualInput.Vertical, -1f, 1f);
            }

            Horizontal = horizontal;
            Vertical = vertical;

            bool virtualJump = useVirtualInput && VirtualInput.ConsumeJumpPressed();
            bool virtualMelee = useVirtualInput && VirtualInput.ConsumeMeleePressed();
            bool virtualReload = useVirtualInput && VirtualInput.ConsumeReloadPressed();

            JumpPressed = Input.GetKeyDown(jumpKey) || virtualJump;
            JumpHeld = Input.GetKey(jumpKey) || (useVirtualInput && VirtualInput.JumpHeld);
            CrouchHeld = vertical < -0.5f || Input.GetKey(KeyCode.LeftControl) ||
                         (useVirtualInput && VirtualInput.CrouchHeld);

            bool mouseFire = useMouseButtons && Input.GetMouseButton(0);
            bool mouseMelee = useMouseButtons && Input.GetMouseButtonDown(1);
            FireHeld = Input.GetKey(fireKey) || mouseFire || (useVirtualInput && VirtualInput.FireHeld);
            MeleePressed = Input.GetKeyDown(meleeKey) || mouseMelee || virtualMelee;

            // La pausa la consume PauseMenu, para que no se pierda el evento.
            PausePressed = Input.GetKeyDown(pauseKey);
            ReloadPressed = Input.GetKeyDown(reloadKey) || virtualReload;
        }

        private void Clear()
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
        }
    }
}
