using UnityEngine;

public static class GameConfig
{
    public const int TargetFrameRateUncapped = -1;
    public const int VSyncDisabled = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ApplyRuntimeSettings()
    {
        Application.targetFrameRate = TargetFrameRateUncapped;
        QualitySettings.vSyncCount = VSyncDisabled;
    }
}
