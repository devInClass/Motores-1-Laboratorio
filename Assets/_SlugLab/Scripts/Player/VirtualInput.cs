using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Entrada virtual que escriben los controles táctiles. El teclado y el
    /// táctil se suman dentro de <see cref="PlayerInput"/>, así que la lógica de
    /// movimiento y combate no sabe de qué dispositivo viene la orden.
    /// </summary>
    public static class VirtualInput
    {
        public static float Horizontal { get; private set; }
        public static float Vertical { get; private set; }
        public static bool JumpHeld { get; private set; }
        public static bool FireHeld { get; private set; }
        public static bool CrouchHeld { get; private set; }

        private static bool _jumpPressed;
        private static bool _meleePressed;
        private static bool _pausePressed;
        private static bool _reloadPressed;

        public static void SetMove(Vector2 value)
        {
            Horizontal = Mathf.Clamp(value.x, -1f, 1f);
            Vertical = Mathf.Clamp(value.y, -1f, 1f);
        }

        public static void SetJump(bool held)
        {
            if (held && !JumpHeld) _jumpPressed = true;
            JumpHeld = held;
        }

        public static void SetFire(bool held) => FireHeld = held;

        public static void SetCrouch(bool held) => CrouchHeld = held;

        public static void PressMelee() => _meleePressed = true;

        public static void PressPause() => _pausePressed = true;

        public static void PressReload() => _reloadPressed = true;

        public static bool ConsumeJumpPressed() => Consume(ref _jumpPressed);

        public static bool ConsumeMeleePressed() => Consume(ref _meleePressed);

        public static bool ConsumePausePressed() => Consume(ref _pausePressed);

        public static bool ConsumeReloadPressed() => Consume(ref _reloadPressed);

        /// <summary>Se llama al cargar escena y al salir de play mode.</summary>
        public static void Reset()
        {
            Horizontal = 0f;
            Vertical = 0f;
            JumpHeld = false;
            FireHeld = false;
            CrouchHeld = false;
            _jumpPressed = false;
            _meleePressed = false;
            _pausePressed = false;
            _reloadPressed = false;
        }

        private static bool Consume(ref bool flag)
        {
            bool value = flag;
            flag = false;
            return value;
        }
    }
}
