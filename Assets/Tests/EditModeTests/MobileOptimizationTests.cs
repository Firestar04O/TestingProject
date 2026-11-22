using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MobileOptimizationTests
{
    private const string MobilePipelinePath = "Assets/Settings/Mobile_RPAsset.asset";

    private static readonly object[] AndroidTextureRules =
    {
        new object[] { "Assets/ModelsFiles/Textures/Wall multiset_Diffuse.png", 2048 },
        new object[] { "Assets/ModelsFiles/Textures/Laboratory03_Floor_BaseMap.png", 1024 },
        new object[] { "Assets/ModelsFiles/Textures/Table texture set 1 V2_Diffuse.png", 1024 },
        new object[] { "Assets/ModelsFiles/Textures/Chair Set _Diffuse.png", 1024 }
    };

    [Test]
    public void DisplayedSecondTracker_RefreshesOnlyWhenVisibleSecondChanges()
    {
        var tracker = new DisplayedSecondTracker();

        Assert.That(tracker.TryGetChangedSecond(30f, out int first), Is.True);
        Assert.That(first, Is.EqualTo(30));
        Assert.That(tracker.TryGetChangedSecond(29.9f, out int same), Is.False);
        Assert.That(same, Is.EqualTo(30));
        Assert.That(tracker.TryGetChangedSecond(29f, out int next), Is.True);
        Assert.That(next, Is.EqualTo(29));

        tracker.Reset();
        Assert.That(tracker.TryGetChangedSecond(29f, out _), Is.True);
    }

    [Test]
    public void InteractionTargetCache_ResolvesComponentsOnlyWhenColliderChanges()
    {
        var root = new GameObject("InteractableRoot");
        var child = new GameObject("ColliderChild");
        child.transform.SetParent(root.transform);
        var scannable = root.AddComponent<CacheTestScannable>();
        var collider = child.AddComponent<BoxCollider>();

        try
        {
            var cache = new InteractionTargetCache();

            Assert.That(cache.Refresh(collider), Is.True);
            Assert.That(cache.Interactable, Is.SameAs(scannable));
            Assert.That(cache.Scannable, Is.SameAs(scannable));
            Assert.That(cache.ResolutionCount, Is.EqualTo(1));

            Assert.That(cache.Refresh(collider), Is.False);
            Assert.That(cache.ResolutionCount, Is.EqualTo(1));

            Assert.That(cache.Refresh(null), Is.True);
            Assert.That(cache.Interactable, Is.Null);
            Assert.That(cache.Scannable, Is.Null);
        }
        finally
        {
            Object.DestroyImmediate(root);
        }
    }

    [Test]
    public void MobilePipeline_UsesConservativeRenderingBudget()
    {
        var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(MobilePipelinePath);
        Assert.That(pipeline, Is.Not.Null, "No se encontró el perfil URP móvil.");

        var serialized = new SerializedObject(pipeline);
        Assert.That(serialized.FindProperty("m_RenderScale").floatValue, Is.LessThanOrEqualTo(0.8f));
        Assert.That(serialized.FindProperty("m_RequireDepthTexture").boolValue, Is.False);
        Assert.That(serialized.FindProperty("m_RequireOpaqueTexture").boolValue, Is.False);
        Assert.That(serialized.FindProperty("m_ShadowDistance").floatValue, Is.LessThanOrEqualTo(30f));
        Assert.That(serialized.FindProperty("m_ShadowCascadeCount").intValue, Is.LessThanOrEqualTo(1));
        Assert.That(serialized.FindProperty("m_AdditionalLightShadowsSupported").boolValue, Is.False);
        Assert.That(serialized.FindProperty("m_SoftShadowsSupported").boolValue, Is.False);
        Assert.That(serialized.FindProperty("m_UseSRPBatcher").boolValue, Is.True);
    }

    [TestCaseSource(nameof(AndroidTextureRules))]
    public void AndroidTextureOverride_UsesAstcAndExpectedLimit(string assetPath, int expectedMaxSize)
    {
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        Assert.That(importer, Is.Not.Null, $"No se encontró el TextureImporter de {assetPath}.");

        TextureImporterPlatformSettings settings = importer.GetPlatformTextureSettings("Android");
        Assert.That(settings.overridden, Is.True, $"Falta el override Android de {assetPath}.");
        Assert.That(settings.maxTextureSize, Is.EqualTo(expectedMaxSize));
        Assert.That(settings.format, Is.EqualTo(TextureImporterFormat.ASTC_6x6));
        Assert.That(settings.textureCompression, Is.EqualTo(TextureImporterCompression.Compressed));
    }
}

public sealed class CacheTestScannable : MonoBehaviour, IScannable
{
    public string GetInteractionPrompt() => "Interactuar";
    public void Interact() { }
    public string GetScanPrompt() => "Escanear";
    public void OnScanStarted() { }
    public void OnScanCancelled() { }
    public void OnScanCompleted() { }
    public bool IsSafe() => true;
}
