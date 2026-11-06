using UnityEngine;
using UnityEngine.EventSystems;

namespace SlugLab.Player
{
    /// <summary>Acciones que puede disparar un botón táctil.</summary>
    public enum VirtualButtonAction
    {
        Jump,
        Fire,
        Melee,
        Pause,
        Reload
    }

    /// <summary>
    /// Botón en pantalla que escribe en <see cref="VirtualInput"/>. Usa los
    /// eventos del sistema de UI, así que funciona con ratón y con varios dedos.
    /// </summary>
    public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private VirtualButtonAction action = VirtualButtonAction.Jump;

        public void Configure(VirtualButtonAction newAction)
        {
            action = newAction;
        }

        public void OnPointerDown(PointerEventData eventData) => Press();

        public void OnPointerUp(PointerEventData eventData) => Release();

        public void OnPointerExit(PointerEventData eventData)
        {
            if (action == VirtualButtonAction.Jump || action == VirtualButtonAction.Fire) Release();
        }

        private void OnDisable() => Release();

        private void Press()
        {
            switch (action)
            {
                case VirtualButtonAction.Jump:
                    VirtualInput.SetJump(true);
                    break;
                case VirtualButtonAction.Fire:
                    VirtualInput.SetFire(true);
                    break;
                case VirtualButtonAction.Melee:
                    VirtualInput.PressMelee();
                    break;
                case VirtualButtonAction.Pause:
                    VirtualInput.PressPause();
                    break;
                case VirtualButtonAction.Reload:
                    VirtualInput.PressReload();
                    break;
            }
        }

        private void Release()
        {
            switch (action)
            {
                case VirtualButtonAction.Jump:
                    VirtualInput.SetJump(false);
                    break;
                case VirtualButtonAction.Fire:
                    VirtualInput.SetFire(false);
                    break;
            }
        }
    }
}
