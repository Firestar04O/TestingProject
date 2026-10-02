using TMPro;
using UnityEngine;

public class LoginUI : MonoBehaviour
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_InputField passwordInput;

    [SerializeField] AuthenticationManager authManager;
    [SerializeField] SceneController sceneController;

    public void Login()
    {
        bool success = authManager.ValidateLogin(
            usernameInput.text,
            passwordInput.text
        );

        if (success)
        {
            sceneController.GoToScene("Gameplay 1");
        }
        else
        {
            Debug.Log("Login incorrecto");
        }
    }
}
