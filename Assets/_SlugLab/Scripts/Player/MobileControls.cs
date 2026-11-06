using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SlugLab.Player
{
    /// <summary>
    /// Construye los controles táctiles en tiempo de ejecución (joystick y
    /// botones) y los conecta a <see cref="VirtualInput"/>. Se crea solo en
    /// dispositivos móviles; en el editor se puede forzar para probar.
    /// </summary>
    [DisallowMultipleComponent]
    public class MobileControls : MonoBehaviour
    {
        private static MobileControls _instance;
        private static Font _font;

        [SerializeField] private bool forceOnDesktop;

        private GameObject _root;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            if (!forceOnDesktop && !Application.isMobilePlatform)
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
            if (!Application.isMobilePlatform || _instance != null) return;

            var go = new GameObject("ControlesMoviles");
            go.AddComponent<MobileControls>();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        private void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            VirtualInput.Reset();
            RefreshVisibility();
        }

        private void RefreshVisibility()
        {
            if (_root == null) return;
            _root.SetActive(FindFirstObjectByType<PlayerInput>() != null);
        }

        private void Build()
        {
            EnsureEventSystem();

            var canvas = gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 200;

            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            gameObject.AddComponent<GraphicRaycaster>();

            var rootGo = new GameObject("Controles", typeof(RectTransform));
            var root = (RectTransform)rootGo.transform;
            root.SetParent(transform, false);
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
            rootGo.AddComponent<UI.SafeAreaFitter>();
            _root = rootGo;

            RectTransform joystickBase = Panel(root, "Joystick", new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(190f, 190f), new Vector2(300f, 300f), new Color(1f, 1f, 1f, 0.16f));

            RectTransform knob = Panel(joystickBase, "Knob", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(120f, 120f), new Color(1f, 1f, 1f, 0.34f));

            var joystick = joystickBase.gameObject.AddComponent<TouchJoystick>();
            joystick.Configure(knob, 78f);

            Button(root, "Saltar", "SALTO", VirtualButtonAction.Jump,
                new Vector2(1f, 0f), new Vector2(-170f, 200f), new Vector2(190f, 190f), new Color(0.4f, 0.9f, 0.7f, 0.32f));

            Button(root, "Disparar", "FUEGO", VirtualButtonAction.Fire,
                new Vector2(1f, 0f), new Vector2(-390f, 150f), new Vector2(175f, 175f), new Color(1f, 0.7f, 0.3f, 0.32f));

            Button(root, "Cuchillo", "K", VirtualButtonAction.Melee,
                new Vector2(1f, 0f), new Vector2(-140f, 420f), new Vector2(110f, 110f), new Color(0.8f, 0.8f, 1f, 0.3f));

            Button(root, "Recargar", "R", VirtualButtonAction.Reload,
                new Vector2(1f, 0f), new Vector2(-270f, 420f), new Vector2(100f, 100f), new Color(0.9f, 0.9f, 0.9f, 0.28f));

            Button(root, "Pausa", "II", VirtualButtonAction.Pause,
                new Vector2(1f, 1f), new Vector2(-80f, -80f), new Vector2(100f, 100f), new Color(1f, 1f, 1f, 0.3f));

            RefreshVisibility();
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current != null) return;

            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            DontDestroyOnLoad(go);
        }

        private static RectTransform Panel(Transform parent, string name, Vector2 anchor, Vector2 pivot,
            Vector2 position, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            var image = go.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
            return rect;
        }

        private static void Button(Transform parent, string name, string label, VirtualButtonAction action,
            Vector2 anchor, Vector2 position, Vector2 size, Color color)
        {
            RectTransform rect = Panel(parent, name, anchor, anchor, position, size, color);
            rect.gameObject.AddComponent<TouchButton>().Configure(action);

            var textGo = new GameObject("Texto", typeof(RectTransform));
            var textRect = (RectTransform)textGo.transform;
            textRect.SetParent(rect, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            var text = textGo.AddComponent<Text>();
            text.font = UIFont;
            text.text = label;
            text.fontSize = 32;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = new Color(1f, 1f, 1f, 0.9f);
            text.raycastTarget = false;
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
