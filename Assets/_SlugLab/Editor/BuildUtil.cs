using System;
using System.Collections.Generic;
using SlugLab.Boss;
using SlugLab.Combat;
using SlugLab.Core;
using SlugLab.Enemies;
using SlugLab.Level;
using SlugLab.Player;
using SlugLab.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SlugLab.EditorTools
{
    /// <summary>
    /// Ladrillos con los que se construyen las escenas del curso: cámara, fondo,
    /// suelo, props, HUD y entidades. Tenerlo separado del "qué escena" permite
    /// crear los laboratorios con muy poco código.
    /// </summary>
    public static class BuildUtil
    {
        public const string ArtRoot = "Assets/_SlugLab/Art";
        public const string PrefabRoot = "Assets/_SlugLab/Prefabs";
        public const string SceneRoot = "Assets/_SlugLab/Scenes";
        public const string DataRoot = "Assets/_SlugLab/Settings/Datos";
        public const string ClipRoot = DataRoot + "/Animaciones";
        public const string TileRoot = DataRoot + "/Tiles";

        public const float Ppu = 32f;
        public const float CameraSize = 5.6f;

        private static readonly Dictionary<string, Sprite> Sprites = new Dictionary<string, Sprite>();
        private static Font _font;

        // ------------------------------------------------------------------
        // Carpetas y assets
        // ------------------------------------------------------------------

        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            string[] parts = path.Split('/');
            string current = parts[0];

            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }

        public static Sprite S(string relative)
        {
            if (Sprites.TryGetValue(relative, out Sprite cached)) return cached;

            string path = $"{ArtRoot}/Sprites/{relative}.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite == null) Debug.LogError($"[Slug Lab] Falta el sprite {path}");
            Sprites[relative] = sprite;
            return sprite;
        }

        public static Sprite[] Seq(string folder, string prefix, int count)
        {
            var frames = new Sprite[count];
            for (int i = 0; i < count; i++) frames[i] = S($"{folder}/{prefix}{i}");
            return frames;
        }

        public static AudioClip A(string relative)
        {
            return AssetDatabase.LoadAssetAtPath<AudioClip>($"Assets/_SlugLab/Resources/{relative}.wav");
        }

        public static Tile NewTile(string name, Sprite sprite, Tile.ColliderType colliderType)
        {
            EnsureFolder(TileRoot);
            string path = $"{TileRoot}/{name}.asset";
            AssetDatabase.DeleteAsset(path);

            var tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            tile.colliderType = colliderType;
            tile.name = name;

            if (sprite == null) Debug.LogError($"[Slug Lab] El tile '{name}' se creó sin sprite.");

            AssetDatabase.CreateAsset(tile, path);
            return tile;
        }

        // ------------------------------------------------------------------
        // Animación
        // ------------------------------------------------------------------

        public static AnimationClip Clip(string name, Sprite[] frames, float fps, bool loop = true)
        {
            EnsureFolder(ClipRoot);
            string path = $"{ClipRoot}/{name}.anim";
            AssetDatabase.DeleteAsset(path);

            var clip = new AnimationClip { frameRate = fps, name = name };

            if (frames != null && frames.Length > 0)
            {
                var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
                var keys = new ObjectReferenceKeyframe[frames.Length];

                for (int i = 0; i < frames.Length; i++)
                {
                    keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = frames[i] };
                }

                AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
            }
            else
            {
                Debug.LogWarning($"[Slug Lab] Clip '{name}' sin frames.");
            }

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }

        public static AnimatorController NewController(string name)
        {
            EnsureFolder(ClipRoot);
            string path = $"{ClipRoot}/{name}.controller";
            AssetDatabase.DeleteAsset(path);
            return AnimatorController.CreateAnimatorControllerAtPath(path);
        }

        public static AnimatorState State(AnimatorStateMachine machine, string name, AnimationClip clip, bool isDefault = false)
        {
            AnimatorState state = machine.AddState(name);
            state.motion = clip;
            state.writeDefaultValues = true;
            if (isDefault) machine.defaultState = state;
            return state;
        }

        public static void Link(AnimatorState from, AnimatorState to, float duration = 0.05f, bool hasExitTime = false,
            params (AnimatorConditionMode mode, string parameter, float threshold)[] conditions)
        {
            AnimatorStateTransition transition = from.AddTransition(to);
            transition.hasExitTime = hasExitTime;
            transition.exitTime = 1f;
            transition.duration = duration;
            transition.hasFixedDuration = true;
            transition.canTransitionToSelf = false;

            foreach (var condition in conditions)
                transition.AddCondition(condition.mode, condition.threshold, condition.parameter);
        }

        public static void AnyLink(AnimatorStateMachine machine, AnimatorState to, float duration = 0.05f,
            params (AnimatorConditionMode mode, string parameter, float threshold)[] conditions)
        {
            AnimatorStateTransition transition = machine.AddAnyStateTransition(to);
            transition.hasExitTime = false;
            transition.duration = duration;
            transition.hasFixedDuration = true;
            transition.canTransitionToSelf = false;

            foreach (var condition in conditions)
                transition.AddCondition(condition.mode, condition.threshold, condition.parameter);
        }

        // ------------------------------------------------------------------
        // Objetos
        // ------------------------------------------------------------------

        public static GameObject NewSprite(string name, Sprite sprite, string layer, string sortingLayer, int order)
        {
            var go = new GameObject(name);
            if (LayerMask.NameToLayer(layer) >= 0) go.layer = LayerMask.NameToLayer(layer);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = order;
            return go;
        }

        public static GameObject SavePrefab(GameObject instance, string name)
        {
            EnsureFolder(PrefabRoot);
            string path = $"{PrefabRoot}/{name}.prefab";

            GameObject saved = PrefabUtility.SaveAsPrefabAsset(instance, path, out bool success);
            Object.DestroyImmediate(instance);

            if (!success || saved == null) Debug.LogError($"[Slug Lab] No se pudo guardar el prefab {path}");
            return saved;
        }

        public static GameObject Place(GameObject prefab, Vector3 position)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.transform.position = position;
            return instance;
        }

        public static GameObject Empty(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent, false);
            return go;
        }

        // ------------------------------------------------------------------
        // Cámara y fondo
        // ------------------------------------------------------------------

        public static CameraFollow2D CreateCamera(Vector2 minBounds, Vector2 maxBounds)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            go.transform.position = new Vector3(minBounds.x, minBounds.y, -10f);

            var camera = go.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = CameraSize;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.05f, 0.06f, 0.11f);
            camera.nearClipPlane = -20f;
            camera.farClipPlane = 100f;

            go.AddComponent<AudioListener>();

            var follow = go.AddComponent<CameraFollow2D>();
            Binder.Set(follow, "useBounds", true);
            Binder.Set(follow, "minBounds", minBounds);
            Binder.Set(follow, "maxBounds", maxBounds);
            Binder.Set(follow, "orthographicSize", CameraSize);
            return follow;
        }

        public static void CreateBackdrop(int width, bool parallax = true)
        {
            float centerX = width * 0.5f;
            float scale = Mathf.Max(3f, width / 15f);

            GameObject sky = NewSprite("Fondo_Cielo", LoadBackground("bg_sky"), "Default", "Fondo", -30);
            sky.transform.position = new Vector3(centerX, 4f, 30f);
            sky.transform.localScale = new Vector3(scale, scale, 1f);

            GameObject far = NewSprite("Fondo_Montanas", LoadBackground("bg_far"), "Default", "Parallax", -20);
            far.transform.position = new Vector3(centerX, 3.2f, 20f);
            far.transform.localScale = new Vector3(scale * 0.9f, scale * 0.9f, 1f);
            if (parallax) Binder.Set(far.AddComponent<ParallaxLayer>(), "factor", 0.15f);

            GameObject mid = NewSprite("Fondo_Cantina", LoadBackground("bg_mid"), "Default", "Parallax", -10);
            mid.transform.position = new Vector3(centerX, 2.4f, 10f);
            mid.transform.localScale = new Vector3(scale * 0.85f, scale * 0.85f, 1f);
            if (parallax) Binder.Set(mid.AddComponent<ParallaxLayer>(), "factor", 0.35f);
        }

        private static Sprite LoadBackground(string name)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtRoot}/Fondos/{name}.png");
        }

        // ------------------------------------------------------------------
        // Nivel
        // ------------------------------------------------------------------

        public static Tilemap CreateGround(int width, (int start, int end)[] gaps)
        {
            Tile topTile = AssetDatabase.LoadAssetAtPath<Tile>($"{TileRoot}/tile_ground_top.asset");
            Tile midTile = AssetDatabase.LoadAssetAtPath<Tile>($"{TileRoot}/tile_ground_mid.asset");

            if (topTile == null || midTile == null)
            {
                Debug.LogError("[Slug Lab] No se encontraron los tiles de suelo. Ejecuta primero 'Generar juego completo'.");
                return null;
            }

            var gridGo = new GameObject("Grid");
            gridGo.transform.position = new Vector3(0f, -0.5f, 0f);
            var grid = gridGo.AddComponent<Grid>();
            grid.cellSize = Vector3.one;

            var tilemapGo = new GameObject("Suelo");
            tilemapGo.transform.SetParent(gridGo.transform, false);
            tilemapGo.layer = LayerMask.NameToLayer("Ground");

            var tilemap = tilemapGo.AddComponent<Tilemap>();
            var renderer = tilemapGo.AddComponent<TilemapRenderer>();
            renderer.sortingLayerName = "Suelo";

            var collider = tilemapGo.AddComponent<TilemapCollider2D>();

            for (int x = 0; x < width; x++)
            {
                bool inGap = false;
                if (gaps != null)
                {
                    foreach ((int start, int end) in gaps)
                    {
                        if (x >= start && x <= end) { inGap = true; break; }
                    }
                }

                if (inGap) continue;

                tilemap.SetTile(new Vector3Int(x, 0, 0), topTile);
                tilemap.SetTile(new Vector3Int(x, -1, 0), midTile);
                tilemap.SetTile(new Vector3Int(x, -2, 0), midTile);
            }

            tilemap.RefreshAllTiles();
            collider.ProcessTilemapChanges();

            // Sin esto, Unity no serializa los tiles al guardar la escena y el
            // nivel se queda sin suelo al cargarlo.
            EditorUtility.SetDirty(tilemap);
            EditorUtility.SetDirty(collider);
            EditorSceneManager.MarkSceneDirty(tilemap.gameObject.scene);

            return tilemap;
        }

        public static GameObject CreatePlatform(string name, Vector3 position, float width, bool oneWay = true)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            go.layer = LayerMask.NameToLayer(oneWay ? "OneWayPlatform" : "Ground");
            go.transform.localScale = new Vector3(width, 1f, 1f);

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = S("Tiles/tile_platform");
            renderer.sortingLayerName = "Props";
            renderer.sortingOrder = -1;

            var box = go.AddComponent<BoxCollider2D>();
            box.size = new Vector2(1f, 0.5f);

            if (oneWay)
            {
                box.usedByEffector = true;
                var effector = go.AddComponent<PlatformEffector2D>();
                effector.useOneWay = true;
                effector.surfaceArc = 170f;
                go.AddComponent<OneWayPlatform>();
            }

            return go;
        }

        public static GameObject CreateMovingPlatform(Vector3 position, float width, Vector3 delta)
        {
            var go = CreatePlatform("Plataforma_Movil", position, width, false);
            var mover = go.AddComponent<MovingPlatform>();
            Binder.Set(mover, "pointB", delta);
            return go;
        }

        public static void CreateProp(string name, Sprite sprite, Vector3 position)
        {
            GameObject prop = NewSprite(name, sprite, "Ground", "Props", 0);
            prop.transform.position = position;
            var box = prop.AddComponent<BoxCollider2D>();
            box.size = new Vector2(sprite.rect.width / Ppu, sprite.rect.height / Ppu);
            box.offset = new Vector2(0f, box.size.y * 0.5f);
        }

        public static void CreateKillZone(int width, float y)
        {
            var go = new GameObject("Zona_Muerte");
            go.transform.position = new Vector3(width * 0.5f, y, 0f);
            go.layer = LayerMask.NameToLayer("Hazard");

            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(width + 20f, 3f);

            go.AddComponent<KillZone>();
        }

        public static Checkpoint CreateCheckpoint(Vector3 position)
        {
            var go = new GameObject("Checkpoint");
            go.transform.position = position;

            var renderer = go.AddComponent<SpriteRenderer>();
            renderer.sprite = S("Props/flag_off");
            renderer.sortingLayerName = "Props";
            renderer.sortingOrder = 2;

            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(1.2f, 2f);
            box.offset = new Vector2(0f, 1f);

            var respawn = new GameObject("Punto_Reaparicion");
            respawn.transform.SetParent(go.transform, false);
            respawn.transform.localPosition = new Vector3(0.6f, 0.2f, 0f);

            var checkpoint = go.AddComponent<Checkpoint>();
            Binder.Set(checkpoint, "flagRenderer", renderer);
            Binder.Set(checkpoint, "respawnPoint", respawn.transform);
            return checkpoint;
        }

        public static LevelGate CreateGate(Vector3 position)
        {
            var root = new GameObject("Puerta");
            root.transform.position = position;

            var barrier = new GameObject("Barrera");
            barrier.transform.SetParent(root.transform, false);
            barrier.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            barrier.transform.localScale = new Vector3(1f, 3f, 1f);
            barrier.layer = LayerMask.NameToLayer("Ground");

            var renderer = barrier.AddComponent<SpriteRenderer>();
            renderer.sprite = S("Tiles/tile_metal");
            renderer.sortingLayerName = "Props";
            renderer.sortingOrder = 1;

            var box = barrier.AddComponent<BoxCollider2D>();
            box.size = new Vector2(0.8f, 1f);

            var lamp = NewSprite("Indicador", S("Props/health_pickup"), "Trigger", "Props", 3);
            lamp.transform.SetParent(root.transform, false);
            lamp.transform.localPosition = new Vector3(0f, 3.2f, 0f);
            lamp.transform.localScale = Vector3.one * 0.6f;

            var gate = root.AddComponent<LevelGate>();
            Binder.Set(gate, "barrier", barrier);
            Binder.Set(gate, "indicator", lamp.GetComponent<SpriteRenderer>());
            return gate;
        }

        public static void CreateGoal(Vector3 position)
        {
            var go = new GameObject("Meta");
            go.transform.position = position;

            var visual = NewSprite("Puerta_Salida", S("Tiles/tile_brick"), "Trigger", "Props", 1);
            visual.transform.SetParent(go.transform, false);
            visual.transform.localPosition = new Vector3(0f, 2f, 0f);
            visual.transform.localScale = new Vector3(2f, 4f, 1f);

            var box = go.AddComponent<BoxCollider2D>();
            box.isTrigger = true;
            box.size = new Vector2(1.8f, 4f);
            box.offset = new Vector2(0f, 2f);

            go.AddComponent<LevelGoal>();
        }

        public static EnemySpawner CreateSpawner(Vector3 position, GameObject enemyPrefab, int count, float spread, LevelGate gate)
        {
            var go = new GameObject("Spawner");
            go.transform.position = position;

            var points = new Transform[count];
            var enemies = new GameObject[count];

            for (int i = 0; i < count; i++)
            {
                var point = new GameObject($"Punto_{i}");
                point.transform.SetParent(go.transform, false);
                point.transform.localPosition = new Vector3(spread * (i - (count - 1) * 0.5f), 0f, 0f);
                points[i] = point.transform;
                enemies[i] = enemyPrefab;
            }

            var spawner = go.AddComponent<EnemySpawner>();
            Binder.Set(spawner, "enemies", enemies);
            Binder.Set(spawner, "spawnPoints", points);
            Binder.Set(spawner, "gate", gate);
            return spawner;
        }

        public static GameFlow CreateFlow(string nextScene)
        {
            var go = new GameObject("GameFlow");
            var flow = go.AddComponent<GameFlow>();
            Binder.Set(flow, "nextSceneName", nextScene);
            Binder.Set(flow, "menuSceneName", "Menu");
            return flow;
        }

        public static void CreateAudio(bool withMusic = true)
        {
            var go = new GameObject("AudioManager");
            go.AddComponent<SlugLab.Audio.AudioManager>();

            if (!withMusic) return;

            AudioClip music = A("Musica/musica_nivel");
            if (music == null) return;

            var musicGo = new GameObject("Musica");
            musicGo.transform.SetParent(go.transform);
            var source = musicGo.AddComponent<AudioSource>();
            source.clip = music;
            source.loop = true;
            source.volume = 0.18f;
            source.playOnAwake = true;
            source.spatialBlend = 0f;
        }

        // ------------------------------------------------------------------
        // Interfaz
        // ------------------------------------------------------------------

        public static Font UIFont
        {
            get
            {
                if (_font != null) return _font;

                _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                if (_font == null) _font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                if (_font == null) _font = Font.CreateDynamicFontFromOSFont("Arial", 32);
                if (_font == null) Debug.LogWarning("[Slug Lab] No se encontró una fuente para la interfaz.");
                return _font;
            }
        }

        public static RectTransform Rect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            return rect;
        }

        public static void Place(Component component, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
            Vector2 anchoredPosition, Vector2 size)
        {
            var rect = (RectTransform)component.transform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = size;
        }

        public static Image NewImage(Transform parent, string name, Sprite sprite, Color color)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        public static Text NewText(Transform parent, string name, string content, int fontSize, TextAnchor alignment, Color color)
        {
            var rect = Rect(name, parent);
            var text = rect.gameObject.AddComponent<Text>();
            text.text = content;
            text.font = UIFont;
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        public static Button NewButton(Transform parent, string name, string label, Color tint, out Text labelText)
        {
            var rect = Rect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = S("UI/button");
            image.type = Image.Type.Sliced;
            image.color = tint;

            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.85f, 1f, 0.95f);
            colors.pressedColor = new Color(0.65f, 0.9f, 0.85f);
            button.colors = colors;

            labelText = NewText(rect, "Texto", label, 34, TextAnchor.MiddleCenter, new Color(0.92f, 1f, 0.97f));
            Place(labelText, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            return button;
        }

        public static Canvas CreateCanvas(string name)
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
            return canvas;
        }

        public static void CreateEventSystem()
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        public static (HUDController hud, PauseMenu pause, EndScreen end, BossHealthBar bossBar) CreateHud(
            PlayerController player, PlayerCombat combat, BossController boss)
        {
            Canvas canvas = CreateCanvas("HUD");
            CreateEventSystem();

            var hud = canvas.gameObject.AddComponent<HUDController>();

            // Vida (5 corazones)
            var pips = new Image[5];
            for (int i = 0; i < pips.Length; i++)
            {
                Image pip = NewImage(canvas.transform, $"Vida_{i}", S("UI/pip_full"), Color.white);
                Place(pip, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                    new Vector2(48f + i * 44f, -40f), new Vector2(38f, 38f));
                pips[i] = pip;
            }

            Text lives = NewText(canvas.transform, "Vidas", "x3", 40, TextAnchor.MiddleLeft, new Color(1f, 0.85f, 0.85f));
            Place(lives, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(48f + 5 * 44f + 8f, -40f), new Vector2(160f, 44f));

            Text score = NewText(canvas.transform, "Puntos", "000000", 44, TextAnchor.MiddleRight, new Color(1f, 0.92f, 0.6f));
            Place(score, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-40f, -40f), new Vector2(320f, 44f));

            Text ammo = NewText(canvas.transform, "Municion", "30", 34, TextAnchor.MiddleLeft, new Color(0.85f, 0.95f, 1f));
            Place(ammo, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(48f, -92f), new Vector2(200f, 40f));

            Binder.Set(hud, "player", player);
            Binder.Set(hud, "combat", combat);
            Binder.Set(hud, "healthPips", pips);
            Binder.Set(hud, "livesText", lives);
            Binder.Set(hud, "scoreText", score);
            Binder.Set(hud, "ammoText", ammo);

            // Barra del jefe
            BossHealthBar bossBar = null;
            if (boss != null)
            {
                var barRoot = Rect("Barra_Jefe", canvas.transform);
                Place(barRoot, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -40f), new Vector2(900f, 90f));

                Image background = NewImage(barRoot, "Fondo", null, new Color(0.05f, 0.05f, 0.08f, 0.85f));
                Place(background, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);

                Image fill = NewImage(barRoot, "Relleno", null, new Color(0.86f, 0.24f, 0.26f));
                fill.type = Image.Type.Filled;
                fill.fillMethod = Image.FillMethod.Horizontal;
                fill.fillOrigin = 0;
                fill.fillAmount = 1f;
                Place(fill, Vector2.zero, Vector2.one, new Vector2(0f, 0.5f), new Vector2(6f, 0f), new Vector2(-12f, -12f));
                ((RectTransform)fill.transform).anchorMin = Vector2.zero;
                ((RectTransform)fill.transform).anchorMax = Vector2.one;

                Text name = NewText(barRoot, "Nombre", boss.DisplayName, 34, TextAnchor.MiddleLeft, Color.white);
                Place(name, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f), new Vector2(10f, 2f), new Vector2(500f, 34f));

                Text phase = NewText(barRoot, "Fase", "FASE 1", 26, TextAnchor.MiddleRight, new Color(1f, 0.85f, 0.6f));
                Place(phase, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-10f, 2f), new Vector2(300f, 30f));

                bossBar = canvas.gameObject.AddComponent<BossHealthBar>();
                Binder.Set(bossBar, "boss", boss);
                Binder.Set(bossBar, "root", barRoot.gameObject);
                Binder.Set(bossBar, "fill", fill);
                Binder.Set(bossBar, "bossName", name);
                Binder.Set(bossBar, "phaseLabel", phase);
            }

            // Pausa
            var pausePanel = NewImage(canvas.transform, "Panel_Pausa", null, new Color(0.02f, 0.02f, 0.05f, 0.82f));
            Place(pausePanel, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            pausePanel.gameObject.SetActive(false);

            Text pauseTitle = NewText(pausePanel.transform, "Titulo", "PAUSA", 92, TextAnchor.MiddleCenter, new Color(0.9f, 1f, 0.95f));
            Place(pauseTitle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 220f), new Vector2(800f, 110f));

            Button resume = NewButton(pausePanel.transform, "Boton_Continuar", "CONTINUAR", Color.white, out _);
            Place(resume, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 40f), new Vector2(420f, 84f));
            Button restartPause = NewButton(pausePanel.transform, "Boton_Reiniciar", "REINICIAR NIVEL", Color.white, out _);
            Place(restartPause, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -70f), new Vector2(420f, 84f));
            Button menuPause = NewButton(pausePanel.transform, "Boton_Menu", "SALIR AL MENU", Color.white, out _);
            Place(menuPause, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -180f), new Vector2(420f, 84f));

            var pause = canvas.gameObject.AddComponent<PauseMenu>();
            Binder.Set(pause, "panel", pausePanel.gameObject);
            Binder.Set(pause, "resumeButton", resume);
            Binder.Set(pause, "restartButton", restartPause);
            Binder.Set(pause, "menuButton", menuPause);

            // Fin de partida
            var endPanel = NewImage(canvas.transform, "Panel_Final", null, new Color(0.01f, 0.01f, 0.03f, 0.9f));
            Place(endPanel, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            endPanel.gameObject.SetActive(false);

            Text endTitle = NewText(endPanel.transform, "Titulo", "MISION CUMPLIDA", 96, TextAnchor.MiddleCenter, new Color(0.95f, 1f, 0.9f));
            Place(endTitle, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 220f), new Vector2(1400f, 120f));

            Text endDetail = NewText(endPanel.transform, "Detalle", "", 40, TextAnchor.MiddleCenter, new Color(0.85f, 0.95f, 1f));
            Place(endDetail, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 80f), new Vector2(1400f, 160f));

            Button restartEnd = NewButton(endPanel.transform, "Boton_Reintentar", "REINTENTAR", Color.white, out _);
            Place(restartEnd, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -90f), new Vector2(420f, 84f));
            Button menuEnd = NewButton(endPanel.transform, "Boton_Menu", "MENU PRINCIPAL", Color.white, out _);
            Place(menuEnd, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -200f), new Vector2(420f, 84f));

            var end = canvas.gameObject.AddComponent<EndScreen>();
            Binder.Set(end, "panel", endPanel.gameObject);
            Binder.Set(end, "titleText", endTitle);
            Binder.Set(end, "detailText", endDetail);
            Binder.Set(end, "restartButton", restartEnd);
            Binder.Set(end, "menuButton", menuEnd);

            return (hud, pause, end, bossBar);
        }

        // ------------------------------------------------------------------
        // Escenas
        // ------------------------------------------------------------------

        public static void SaveScene(UnityEngine.SceneManagement.Scene scene, string sceneName)
        {
            EnsureFolder(SceneRoot);
            EditorSceneManager.SaveScene(scene, $"{SceneRoot}/{sceneName}.unity");
        }

        public static UnityEngine.SceneManagement.Scene NewScene()
        {
            return EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        /// <summary>
        /// Cartel de instrucciones. Se dibuja en <b>Screen Space Overlay</b>:
        /// un canvas en World Space queda detrás de los sprites en URP 2D y el
        /// texto se vuelve ilegible (o invisible) según la profundidad.
        /// </summary>
        public static void AddInstructionSign(Vector3 position, string message)
        {
            Canvas canvas = CreateCanvas("Cartel_Instrucciones");
            canvas.sortingOrder = 40;
            canvas.transform.position = Vector3.zero;

            Image panel = NewImage(canvas.transform, "Panel", null, new Color(0.03f, 0.04f, 0.08f, 0.86f));
            Place(panel, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 34f), new Vector2(1120f, 200f));

            Image strip = NewImage(panel.transform, "Franja", null, new Color(0.45f, 0.9f, 0.75f, 0.95f));
            Place(strip, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, 0f), new Vector2(0f, 4f));

            Text text = NewText(panel.transform, "Mensaje", message, 30, TextAnchor.MiddleLeft,
                new Color(0.96f, 0.98f, 0.86f));
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.lineSpacing = 1.1f;

            var rect = (RectTransform)text.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = new Vector2(30f, 18f);
            rect.offsetMax = new Vector2(-30f, -18f);
        }
    }
}
