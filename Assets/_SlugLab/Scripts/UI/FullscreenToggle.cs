using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SlugLab.UI
{
    /// <summary>
    /// Botón de pantalla completa para WebGL. Se crea solo en el navegador (o
    /// forzado en el editor para probar) y sobrevive a los cambios de escena.
    /// </summary>
    /// <remarks>
    /// En WebGL el navegador solo permite entrar a pantalla completa tras un
    /// gesto del usuario; el clic en este botón cuenta como gesto.
    /// </remarks>
    [DisallowMultipleComponent]
    public class FullscreenToggle : MonoBehaviour
    {
        private static FullscreenToggle _instance;
        private static Font _font;

        [SerializeField] private bool forceOnDesktop;

        private Button _button;
        private Text _label;
        private float _nextCheck;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (!forceOnDesktop && Application.platform != RuntimePlatform.WebGLPlayer)
            {
                gameObject.SetActive(false);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Build();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoSpawn()
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer || _instance != null) return;

            var go = new GameObject("PantallaCompleta");
            go.AddComponent<FullscreenToggle>();
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        private void Update()
        {
            if (_label == null || Time.unscaledTime < _nextCheck) return;
            _nextCheck = Time.unscaledTime + 0.5f;
            UpdateLabel();
        }

        /// <summary>Alterna pantalla completa. Público para el botón y las pruebas.</summary>
        public void Toggle()
        {
            Screen.fullScreen = !Screen.fullScreen;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (_label == null) return;
            _label.text = Screen.fullScreen ? "VENTANA" : "PANTALLA COMPLETA";
        }

        private void Build()
        {
            EnsureEventSystem();

            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 210;

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();

            var buttonGo = new GameObject("Boton", typeof(RectTransform));
            var rect = (RectTransform)buttonGo.transform;
            rect.SetParent(transform, false);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(36f, 36f);
            rect.sizeDelta = new Vector2(340f, 66f);

            var image = buttonGo.AddComponent<Image>();
            image.color = new Color(0.04f, 0.07f, 0.1f, 0.78f);

            _button = buttonGo.AddComponent<Button>();
            _button.targetGraphic = image;
            _button.onClick.AddListener(Toggle);

            var textGo = new GameObject("Texto", typeof(RectTransform));
            var textRect = (RectTransform)textGo.transform;
            textRect.SetParent(rect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            _label = textGo.AddComponent<Text>();
            _label.font = UIFont;
            _label.fontSize = 26;
            _label.alignment = TextAnchor.MiddleCenter;
            _label.color = new Color(0.9f, 1f, 0.95f, 0.92f);
            _label.raycastTarget = false;
            _label.text = Screen.fullScreen ? "VENTANA" : "PANTALLA COMPLETA";
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;

            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(go);
        }

        private static Font UIFont
        {
            get
            {
                if (_font != null) return _font;

                _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (_font == null) _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                if (_font == null) _font = Font.CreateDynamicFontFromOSFont("Arial", 32);
                return _font;
            }
        }
    }
}
