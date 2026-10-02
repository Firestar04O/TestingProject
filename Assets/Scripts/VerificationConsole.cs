using UnityEngine;

public class VerificationConsole : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private VerificationManager verificationManager;

    [Header("Interaction")]
    [SerializeField] private string prompt = "Presiona E para iniciar la verificación.";
    public string GetInteractionPrompt()
    {
        return prompt;
    }
    public void Interact()
    {
        if (verificationManager == null)
        {
            Debug.LogWarning("VerificationManager no asignado en la consola.");
            return;
        }
        verificationManager.StartVerification();
    }
}