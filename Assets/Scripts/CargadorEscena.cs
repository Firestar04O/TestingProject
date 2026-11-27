using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Lab14 - Ejercicio 4 ("Loading, tiempo de carga estimado"), Paso 1.
/// Mide el tiempo real de carga de una escena en PC para, junto a los MB cargados
/// (Profiler → Memory, antes/después), estimar el tiempo equivalente en el HDD de
/// Xbox One (~100 MB/s) que se usa en la Tabla 7.
/// </summary>
public class CargadorEscena : MonoBehaviour
{
    public IEnumerator Cargar(string nombre)
    {
        float t0 = Time.realtimeSinceStartup;
        var op = SceneManager.LoadSceneAsync(nombre);
        while (!op.isDone) yield return null;
        Debug.Log($"Carga en PC: {Time.realtimeSinceStartup - t0:F2} s");
    }
}
