using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace SlugLab.Tests
{
    /// <summary>
    /// Carga prefabs en tests de PlayMode. Solo se usa desde la suite de pruebas.
    /// </summary>
    public static class PrefabTestUtility
    {
        private static readonly Dictionary<string, GameObject> Cache = new Dictionary<string, GameObject>();

        public static GameObject Load(string prefabName)
        {
            if (Cache.TryGetValue(prefabName, out GameObject cached)) return cached;

#if UNITY_EDITOR
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/_SlugLab/Prefabs/{prefabName}.prefab");
            Cache[prefabName] = prefab;
            return prefab;
#else
            return null;
#endif
        }
    }
}
