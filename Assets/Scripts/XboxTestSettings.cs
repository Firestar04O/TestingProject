using UnityEngine;

/// <summary>
/// Lab14 - "Preparación: configurar el build y el Profiler", Paso 3.
/// Fija condiciones de prueba iguales para todos los grupos al medir con el Profiler
/// (sin vSync, resolución fija 1920x1080), siguiendo el procedimiento del laboratorio.
/// Colocar en un GameObject vacío llamado "TestSettings" en la escena usada para medir.
/// </summary>
public class XboxTestSettings : MonoBehaviour
{
    private void Awake()
    {
        QualitySettings.vSyncCount = 0; // sin vSync
        Screen.SetResolution(1920, 1080, true);
    }
}
