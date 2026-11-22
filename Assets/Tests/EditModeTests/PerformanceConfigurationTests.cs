using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PerformanceConfigurationTests
{
    private const string PipelineAssetPath = "Assets/Settings/PC_RPAsset.asset";

    private static readonly string[] InstancedMaterialPaths =
    {
        "Assets/ModelsFiles/Materials/Wall Set 1 T1.mat",
        "Assets/ModelsFiles/Materials/Laboratory03_Floor_BaseMap.mat",
        "Assets/ModelsFiles/Materials/Table Texture Set 1 V2.mat",
        "Assets/ModelsFiles/Materials/Chair set.mat",
        "Assets/ModelsFiles/Materials/Cosmic_Retro_Bedroom_Props.mat"
    };

    [Test]
    public void PcPipeline_UsesSrpBatcherWithoutUnusedCameraCopies()
    {
        var pipeline = LoadPcPipeline();

        Assert.That(pipeline.useSRPBatcher, Is.True,
            "El SRP Batcher debe permanecer activado en el perfil de PC.");
        Assert.That(pipeline.supportsCameraDepthTexture, Is.False,
            "La textura de profundidad global no debe generarse si las escenas no la consumen.");
        Assert.That(pipeline.supportsCameraOpaqueTexture, Is.False,
            "La textura opaca global no debe generar una copia de color innecesaria.");
    }

    [Test]
    public void PcPipeline_UsesIndoorShadowBudget()
    {
        var pipeline = LoadPcPipeline();

        Assert.That(pipeline.shadowDistance, Is.LessThanOrEqualTo(30f),
            "El escenario interior debe limitar la distancia de sombras a 30 metros.");
        Assert.That(pipeline.shadowCascadeCount, Is.LessThanOrEqualTo(2),
            "El perfil de PC debe utilizar como máximo dos cascadas de sombra.");
    }

    [TestCaseSource(nameof(InstancedMaterialPaths))]
    public void RepeatedEnvironmentMaterial_HasGpuInstancingEnabled(string assetPath)
    {
        var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

        Assert.That(material, Is.Not.Null, $"No se encontró el material {assetPath}.");
        Assert.That(material.enableInstancing, Is.True,
            $"El material repetido {assetPath} debe permitir GPU Instancing.");
    }

    private static UniversalRenderPipelineAsset LoadPcPipeline()
    {
        var pipeline = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(PipelineAssetPath);
        Assert.That(pipeline, Is.Not.Null, "No se encontró el perfil URP de PC.");
        return pipeline;
    }
}
