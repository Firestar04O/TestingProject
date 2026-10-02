using UnityEngine;

public class DoorOption : MonoBehaviour, IScannable
{
    [Header("Door Data")]
    [SerializeField] private DoorType doorType;
    [SerializeField] private bool isCorrectDoor;
    [SerializeField] private Sprite evidenceSprite;

    [Header("Messages")]
    [SerializeField] private string interactionPrompt = "E Abrir / F Escanear";
    [SerializeField] private string scanPrompt = "Mantén F para escanear.";

    [Header("References")]
    [SerializeField] private VerificationManager verificationManager;

    public string GetInteractionPrompt()
    {
        return interactionPrompt;
    }

    public void Interact()
    {
        if (verificationManager == null)
        {
            Debug.LogWarning("VerificationManager no asignado.");
            return;
        }

        verificationManager.ChooseDoor(this);
    }

    public string GetScanPrompt()
    {
        return scanPrompt;
    }

    public void OnScanStarted()
    {
        FeedbackUI.Message("Escaneando...", 1f);
    }

    public void OnScanCancelled()
    {
        FeedbackUI.Message("Escaneo cancelado.", 1.5f);
    }

    public void OnScanCompleted()
    {
        if (IsSafe())
            FeedbackUI.Message("Resultado: puerta segura.", 2f);
        else
            FeedbackUI.Message("Resultado: amenaza detectada.", 2f);
    }

    public bool IsSafe()
    {
        return isCorrectDoor;
    }

    public Sprite GetEvidenceSprite()
    {
        return evidenceSprite;
    }

    public bool IsCorrectDoor()
    {
        return isCorrectDoor;
    }

    public DoorType GetDoorType()
    {
        return doorType;
    }

    public void Initialize(DoorType newType, bool correct, Sprite sprite)
    {
        doorType = newType;
        isCorrectDoor = correct;
        evidenceSprite = sprite;
    }

    public void SetVerificationManager(VerificationManager manager)
    {
        verificationManager = manager;
    }
}