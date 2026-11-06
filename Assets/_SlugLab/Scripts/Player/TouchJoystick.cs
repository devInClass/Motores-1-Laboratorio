using UnityEngine;
using UnityEngine.EventSystems;

namespace SlugLab.Player
{
    /// <summary>
    /// Joystick táctil: se arrastra el dedo y escribe en <see cref="VirtualInput"/>.
    /// El desplazamiento se limita a un radio para que no se escape del círculo.
    /// </summary>
    public class TouchJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform knob;
        [SerializeField] private float radius = 70f;

        private Vector2 _value;

        public Vector2 Value => _value;

        public void Configure(RectTransform knobRect, float dragRadius)
        {
            knob = knobRect;
            radius = Mathf.Max(1f, dragRadius);
        }

        public void OnPointerDown(PointerEventData eventData) => UpdateValue(eventData);

        public void OnDrag(PointerEventData eventData) => UpdateValue(eventData);

        public void OnPointerUp(PointerEventData eventData) => Release();

        private void OnDisable() => Release();

        private void UpdateValue(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform, eventData.position, eventData.pressEventCamera, out Vector2 local);

            _value = Vector2.ClampMagnitude(local / radius, 1f);

            if (knob != null) knob.anchoredPosition = _value * radius;

            VirtualInput.SetMove(_value);
        }

        private void Release()
        {
            _value = Vector2.zero;
            if (knob != null) knob.anchoredPosition = Vector2.zero;
            VirtualInput.SetMove(Vector2.zero);
        }
    }
}
