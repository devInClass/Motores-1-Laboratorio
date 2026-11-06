using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Traduce el estado del <see cref="PlayerController"/> y del
    /// <see cref="PlayerCombat"/> a parámetros del Animator.
    /// </summary>
    /// <remarks>
    /// El Animator tiene dos blend trees:
    /// <list type="bullet">
    /// <item><b>Locomotion</b> (1D por <c>Speed01</c>): idle ↔ run.</item>
    /// <item><b>Aim</b> (2D por <c>AimAngle</c> y <c>Speed01</c>): 5 direcciones
    /// de apuntado, de pie o corriendo.</item>
    /// </list>
    /// </remarks>
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerController controller;
        [Tooltip("Hijo con el SpriteRenderer que se voltea al cambiar de dirección.")]
        [SerializeField] private Transform visual;

        private PlayerCombat _combat;

        private static readonly int Speed01Hash = Animator.StringToHash("Speed01");
        private static readonly int AimAngleHash = Animator.StringToHash("AimAngle");
        private static readonly int AimingHash = Animator.StringToHash("Aiming");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private static readonly int CrouchHash = Animator.StringToHash("Crouch");
        private static readonly int DeadHash = Animator.StringToHash("Dead");
        private static readonly int MeleeHash = Animator.StringToHash("Melee");
        private static readonly int HurtHash = Animator.StringToHash("Hurt");

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (controller == null) controller = GetComponentInParent<PlayerController>();
            _combat = GetComponentInParent<PlayerCombat>();
        }

        private void Update()
        {
            if (animator == null || controller == null) return;

            Vector2 velocity = controller.Velocity;
            animator.SetFloat(Speed01Hash, controller.SpeedNormalized);
            animator.SetBool(GroundedHash, controller.IsGrounded);
            animator.SetFloat(VerticalSpeedHash, velocity.y);
            animator.SetBool(CrouchHash, controller.IsCrouching);
            animator.SetBool(DeadHash, controller.CurrentState == PlayerController.State.Dead);

            if (_combat != null)
            {
                animator.SetBool(AimingHash, _combat.IsAiming);
                animator.SetFloat(AimAngleHash, PlayerCombat.AimAngleDegrees(_combat.AimDirection));
            }

            if (visual != null)
            {
                Vector3 scale = visual.localScale;
                scale.x = Mathf.Abs(scale.x) * controller.Facing;
                visual.localScale = scale;
            }
        }

        public void PlayMelee()
        {
            if (animator != null) animator.SetTrigger(MeleeHash);
        }

        public void PlayHurt()
        {
            if (animator != null) animator.SetTrigger(HurtHash);
        }
    }
}
