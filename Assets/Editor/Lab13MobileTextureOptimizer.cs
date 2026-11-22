using UnityEditor;
using UnityEngine;

public static class Lab13MobileTextureOptimizer
{
    private const string AndroidPlatform = "Android";

    private readonly struct TextureRule
    {
        public TextureRule(string path, int maxSize)
        {
            Path = path;
            MaxSize = maxSize;
        }

        public string Path { get; }
        public int MaxSize { get; }
    }

    private static readonly TextureRule[] Rules =
    {
        new TextureRule("Assets/ModelsFiles/Textures/Wall multiset_Diffuse.png", 2048),
        new TextureRule("Assets/ModelsFiles/Textures/Laboratory03_Floor_BaseMap.png", 1024),
        new TextureRule("Assets/ModelsFiles/Textures/Table texture set 1 V2_Diffuse.png", 1024),
        new TextureRule("Assets/ModelsFiles/Textures/Chair Set _Diffuse.png", 1024)
    };

    [MenuItem("TecsupVerse/Optimization/Apply Android Texture Overrides")]
    public static void ApplyAndroidTextureOverrides()
    {
        int changed = 0;

        foreach (TextureRule rule in Rules)
        {
            TextureImporter importer = AssetImporter.GetAtPath(rule.Path) as TextureImporter;

            if (importer == null)
            {
                Debug.LogError($"[Lab13] No se encontró el TextureImporter de {rule.Path}.");
                continue;
            }

            TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings(AndroidPlatform);
            settings.name = AndroidPlatform;
            settings.overridden = true;
            settings.maxTextureSize = rule.MaxSize;
            settings.format = TextureImporterFormat.ASTC_6x6;
            settings.textureCompression = TextureImporterCompression.Compressed;
            settings.compressionQuality = 50;

            importer.SetPlatformTextureSettings(settings);
            importer.SaveAndReimport();
            changed++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[Lab13] Overrides Android aplicados a {changed}/{Rules.Length} texturas.");
    }
}
