using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // --- Tarea de optimización (Tabla 10 / Ejercicio 4 - Lab14): carga asíncrona ---
    // Antes: SceneManager.LoadScene() es síncrono y congela un frame completo mientras
    // carga la escena (más notorio en discos lentos, como el HDD de Xbox One).
    // Ahora: se usa LoadSceneAsync, que no bloquea el frame, y además registra el
    // tiempo real de carga en PC (útil para la Tabla 7 de loading).
    public void GoToScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncRoutine(sceneName));
    }

    private IEnumerator LoadSceneAsyncRoutine(string sceneName)
    {
        float t0 = Time.realtimeSinceStartup;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (operation != null && !operation.isDone)
            yield return null;

        float loadTime = Time.realtimeSinceStartup - t0;
        Debug.Log($"Carga de escena '{sceneName}' en PC: {loadTime:F2} s");
    }

    //public void RestartLevel()
    //{
    //    Scene currentScene = SceneManager.GetActiveScene();
    //    SceneManager.LoadScene(currentScene.name);
    //}
    public void QuitGame()
    {
        Application.Quit();
    }
}
