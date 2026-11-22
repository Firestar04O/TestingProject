using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private AuthenticationManager authManager;
    [SerializeField] private SceneController sceneController;

    private void OnEnable()
    {
        if (usernameInput != null)
        {
            usernameInput.Select();
            usernameInput.ActivateInputField();
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
            return;

        if (keyboard.tabKey.wasPressedThisFrame)
        {
            bool shiftPressed = keyboard.leftShiftKey.isPressed || keyboard.rightShiftKey.isPressed;
            MoveFocus(shiftPressed);
        }

        if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame)
            Login();
    }

    private void MoveFocus(bool reverse)
    {
        GameObject selected = EventSystem.current != null
            ? EventSystem.current.currentSelectedGameObject
            : null;

        TMP_InputField target = selected == passwordInput?.gameObject
            ? usernameInput
            : passwordInput;

        if (reverse)
            target = selected == usernameInput?.gameObject ? passwordInput : usernameInput;

        if (target == null)
            return;

        target.Select();
        target.ActivateInputField();
    }

    public void Login()
    {
        if (usernameInput == null || passwordInput == null || authManager == null || sceneController == null)
        {
            SetStatus("La pantalla de acceso no está configurada correctamente.");
            return;
        }

        if (string.IsNullOrWhiteSpace(usernameInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            SetStatus("Completa el usuario y la contraseña.");
            return;
        }

        if (loginButton != null)
            loginButton.interactable = false;

        bool success = authManager.ValidateLogin(
            usernameInput.text.Trim(),
            passwordInput.text
        );

        if (success)
        {
            SetStatus("Acceso validado. Cargando entorno...");
            sceneController.GoToScene("TesisProject");
        }
        else
        {
            SetStatus("Usuario o contraseña incorrectos.");
            passwordInput.text = string.Empty;
            passwordInput.Select();
            passwordInput.ActivateInputField();

            if (loginButton != null)
                loginButton.interactable = true;
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;

        Debug.Log(message);
    }
}
