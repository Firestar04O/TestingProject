using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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