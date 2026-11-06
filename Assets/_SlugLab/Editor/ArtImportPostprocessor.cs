using UnityEditor;
using UnityEngine;

namespace SlugLab.EditorTools
{
    /// <summary>
    /// Aplica las reglas de importación del arte del curso:
    /// 32 píxeles = 1 unidad, filtro Point (pixel art nítido), sin compresión,
    /// sin mipmaps y pivote abajo-centro para personajes.
    /// </summary>
    /// <remarks>
    /// Esto es la versión "automática" de lo que se enseña en el Módulo 1:
    /// si el ajuste está en el postprocesador, ningún alumno puede olvidarlo.
    /// </remarks>
    public class ArtImportPostprocessor : AssetPostprocessor
    {
        public const float PixelsPerUnit = 32f;

        private static readonly string[] BottomPivotFolders =
        {
            "/Personajes/", "/Enemigos/", "/Jefe/", "/Props/"
        };

        private void OnPreprocessTexture()
        {
            string path = assetPath.Replace('\\', '/');
            if (!path.Contains("/_SlugLab/Art/")) return;

            TextureImporter importer = (TextureImporter)assetImporter;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.maxTextureSize = 2048;

            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Center;
            settings.spriteMeshType = SpriteMeshType.FullRect;

            foreach (string folder in BottomPivotFolders)
            {
                if (path.Contains(folder))
                {
                    settings.spriteAlignment = (int)SpriteAlignment.BottomCenter;
                    break;
                }
            }

            importer.SetTextureSettings(settings);
        }

        private void OnPreprocessAudio()
        {
            string path = assetPath.Replace('\\', '/');
            if (!path.Contains("/_SlugLab/Resources/")) return;

            AudioImporter importer = (AudioImporter)assetImporter;
            AudioImporterSampleSettings settings = importer.defaultSampleSettings;
            settings.loadType = AudioClipLoadType.DecompressOnLoad;
            settings.compressionFormat = AudioCompressionFormat.PCM;
            importer.defaultSampleSettings = settings;
        }
    }
}
