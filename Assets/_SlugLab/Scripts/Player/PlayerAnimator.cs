using UnityEngine;

namespace SlugLab.Player
{
    /// <summary>
    /// Traduce el estado del <see cref="PlayerController"/> a parámetros del
    /// Animator. El controlador no sabe nada de animaciones y el animador no
    /// sabe nada de física: cada uno hace una cosa.
    /// </summary>
    public class PlayerAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerController controller;
        [Tooltip("Hijo con el SpriteRenderer que se voltea al cambiar de dirección.")]
        [SerializeField] private Transform visual;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int GroundedHash = Animator.StringToHash("Grounded");
        private static readonly int VerticalSpeedHash = Animator.StringToHash("VerticalSpeed");
        private static readonly int CrouchHash = Animator.StringToHash("Crouch");
        private static readonly int DeadHash = Animator.StringToHash("Dead");
        private static readonly int ShootHash = Animator.StringToHash("Shoot");
        private static readonly int MeleeHash = Animator.StringToHash("Melee");
        private static readonly int HurtHash = Animator.StringToHash("Hurt");

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (controller == null) controller = GetComponentInParent<PlayerController>();
        }

        private void Update()
        {
            if (animator == null || controller == null) return;

            Vector2 velocity = controller.Velocity;
            animator.SetFloat(SpeedHash, Mathf.Abs(velocity.x));
            animator.SetBool(GroundedHash, controller.IsGrounded);
            animator.SetFloat(VerticalSpeedHash, velocity.y);
            animator.SetBool(CrouchHash, controller.IsCrouching);
            animator.SetBool(DeadHash, controller.CurrentState == PlayerController.State.Dead);

            if (visual != null)
            {
                Vector3 scale = visual.localScale;
                scale.x = Mathf.Abs(scale.x) * controller.Facing;
                visual.localScale = scale;
            }
        }

        public void PlayShoot()
        {
            if (animator != null) animator.SetTrigger(ShootHash);
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
