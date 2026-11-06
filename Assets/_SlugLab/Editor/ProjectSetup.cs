using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SlugLab.EditorTools
{
    /// <summary>
    /// Configura el proyecto (tags, capas, sorting layers, build settings) para
    /// que el alumnado no tenga que hacerlo a mano antes de la primera clase.
    /// </summary>
    public static class ProjectSetup
    {
        public static readonly string[] RequiredTags = { "Player", "Enemy", "Boss" };

        public static readonly string[] RequiredLayers =
        {
            "Player", "Enemy", "PlayerAttack", "EnemyAttack",
            "Ground", "OneWayPlatform", "Hazard", "Trigger"
        };

        public static readonly string[] RequiredSortingLayers =
        {
            "Fondo", "Parallax", "Suelo", "Props", "Personajes", "VFX", "UI"
        };

        public const string ArtFolder = "Assets/_SlugLab/Art";

        [MenuItem("Slug Lab/1. Configurar proyecto", priority = 1)]
        public static void ConfigureAll()
        {
            ConfigureTags();
            ConfigureLayers();
            ConfigureSortingLayers();
            ConfigurePlayerSettings();
            ReimportArt();
            Debug.Log("[Slug Lab] Proyecto configurado: tags, capas, sorting layers y ajustes de arte.");
        }

        private static SerializedObject TagManager
        {
            get
            {
                Object[] assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
                if (assets == null || assets.Length == 0)
                    throw new System.Exception("No se pudo abrir ProjectSettings/TagManager.asset");
                return new SerializedObject(assets[0]);
            }
        }

        public static void ConfigureTags()
        {
            SerializedObject so = TagManager;
            SerializedProperty tags = so.FindProperty("tags");

            foreach (string tag in RequiredTags)
            {
                bool exists = Enumerable.Range(0, tags.arraySize)
                    .Any(i => tags.GetArrayElementAtIndex(i).stringValue == tag);
                if (exists) continue;

                tags.InsertArrayElementAtIndex(tags.arraySize);
                tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
            }

            so.ApplyModifiedProperties();
        }

        public static void ConfigureLayers()
        {
            SerializedObject so = TagManager;
            SerializedProperty layers = so.FindProperty("layers");

            foreach (string layerName in RequiredLayers)
            {
                bool exists = Enumerable.Range(0, layers.arraySize)
                    .Any(i => layers.GetArrayElementAtIndex(i).stringValue == layerName);
                if (exists) continue;

                int free = -1;
                for (int i = 8; i < layers.arraySize; i++)
                {
                    if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                    {
                        free = i;
                        break;
                    }
                }

                if (free < 0)
                {
                    Debug.LogError("[Slug Lab] No quedan capas libres (8-31).");
                    break;
                }

                layers.GetArrayElementAtIndex(free).stringValue = layerName;
            }

            so.ApplyModifiedProperties();
        }

        public static void ConfigureSortingLayers()
        {
            SerializedObject so = TagManager;
            SerializedProperty sortingLayers = so.FindProperty("m_SortingLayers");

            foreach (string layerName in RequiredSortingLayers)
            {
                bool exists = Enumerable.Range(0, sortingLayers.arraySize)
                    .Any(i => sortingLayers.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == layerName);
                if (exists) continue;

                sortingLayers.InsertArrayElementAtIndex(sortingLayers.arraySize);
                SerializedProperty element = sortingLayers.GetArrayElementAtIndex(sortingLayers.arraySize - 1);
                element.FindPropertyRelative("name").stringValue = layerName;
                element.FindPropertyRelative("uniqueID").intValue = Mathf.Abs(layerName.GetHashCode());
            }

            so.ApplyModifiedProperties();
        }

        public static void ConfigurePlayerSettings()
        {
            PlayerSettings.companyName = "Slug Lab";
            PlayerSettings.productName = "Slug Lab - Motores de Videojuegos I";
            PlayerSettings.defaultScreenWidth = 1920;
            PlayerSettings.defaultScreenHeight = 1080;
            PlayerSettings.runInBackground = true;
            PlayerSettings.resizableWindow = true;
        }

        /// <summary>Fuerza la reimportación del arte para aplicar el postprocesador.</summary>
        public static void ReimportArt()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { ArtFolder });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }

        [MenuItem("Slug Lab/2. Verificar configuración", priority = 2)]
        public static void Verify()
        {
            string missing = string.Empty;

            foreach (string tag in RequiredTags)
                if (!UnityEditorInternal.InternalEditorUtility.tags.Contains(tag)) missing += $"tag {tag}; ";

            foreach (string layer in RequiredLayers)
                if (LayerMask.NameToLayer(layer) < 0) missing += $"capa {layer}; ";

            if (missing.Length == 0)
                Debug.Log("[Slug Lab] Configuración correcta.");
            else
                Debug.LogWarning("[Slug Lab] Falta configurar: " + missing);
        }
    }
}
