using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace SlugLab.Tests.EditMode
{
    /// <summary>
    /// TC-24 a TC-30: el proyecto está bien configurado.
    /// Estos tests son la red de seguridad del curso: si alguien toca las capas,
    /// los ajustes de importación o el build, la suite avisa.
    /// </summary>
    [TestFixture]
    public class ProjectSanityTests
    {
        private static readonly string[] Layers = { "Player", "Enemy", "PlayerAttack", "EnemyAttack", "Ground", "OneWayPlatform", "Hazard", "Trigger" };
        private static readonly string[] Sorting = { "Fondo", "Parallax", "Suelo", "Props", "Personajes", "VFX", "UI" };
        private static readonly string[] Scenes = { "Menu", "Nivel1_Cantina", "Nivel2_Jefe", "Lab_01_Sprites", "Lab_03_Fisica", "Lab_04_Combate", "Lab_05_IA", "Lab_06_Niveles", "Lab_07_UI", "Lab_08_Jefe" };
        private static readonly string[] Prefabs = { "Player", "Enemigo_Soldado", "Enemigo_Granadero", "Jefe", "Bala_Jugador", "Bala_Enemigo" };

        [Test]
        public void TC24_RequiredLayersExist()
        {
            foreach (string layer in Layers)
                Assert.GreaterOrEqual(LayerMask.NameToLayer(layer), 0, $"Falta la capa '{layer}'.");
        }

        [Test]
        public void TC25_RequiredSortingLayersExist()
        {
            string[] names = SortingLayer.layers.Select(l => l.name).ToArray();
            foreach (string layer in Sorting)
                CollectionAssert.Contains(names, layer, $"Falta la sorting layer '{layer}'.");
        }

        [Test]
        public void TC26_PlayerTagExists()
        {
            Assert.IsTrue(UnityEditorInternal.InternalEditorUtility.tags.Contains("Player"));
            Assert.IsTrue(UnityEditorInternal.InternalEditorUtility.tags.Contains("Enemy"));
        }

        [Test]
        public void TC27_BuildSettings_StartWithMenuAndLevels()
        {
            string[] scenes = EditorBuildSettings.scenes.Select(s => System.IO.Path.GetFileNameWithoutExtension(s.path)).ToArray();

            Assert.GreaterOrEqual(scenes.Length, 3, "Deben estar las escenas del juego en Build Settings.");
            Assert.AreEqual("Menu", scenes[0]);
            Assert.AreEqual("Nivel1_Cantina", scenes[1]);
            Assert.AreEqual("Nivel2_Jefe", scenes[2]);
        }

        [Test]
        public void TC28_AllCourseScenesExist()
        {
            foreach (string scene in Scenes)
            {
                string path = $"Assets/_SlugLab/Scenes/{scene}.unity";
                Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<SceneAsset>(path), $"Falta la escena {path}.");
            }
        }

        [Test]
        public void TC29_CharacterSprites_UseCourseImportSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/_SlugLab/Art/Sprites/Personajes" });
            Assert.IsNotEmpty(guids, "No hay sprites de personajes.");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var importer = (TextureImporter)AssetImporter.GetAtPath(path);

                Assert.AreEqual(TextureImporterType.Sprite, importer.textureType, path);
                Assert.AreEqual(32f, importer.spritePixelsPerUnit, 0.01f, $"{path}: PPU debe ser 32.");
                Assert.AreEqual(FilterMode.Point, importer.filterMode, $"{path}: filtro debe ser Point.");
                Assert.AreEqual(TextureImporterCompression.Uncompressed, importer.textureCompression, path);
            }
        }

        [Test]
        public void TC30_PrefabsExist()
        {
            foreach (string prefab in Prefabs)
            {
                string path = $"Assets/_SlugLab/Prefabs/{prefab}.prefab";
                Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<GameObject>(path), $"Falta el prefab {path}.");
            }
        }
    }
}
