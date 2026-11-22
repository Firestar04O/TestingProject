using UnityEngine;

public class LockedLabDoor : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private SlidingDoorGroup slidingDoor;

    [Header("State")]
    [SerializeField] private bool unlocked;

    [Header("Messages")]
    [SerializeField] private string lockedPrompt = "Laboratorio bloqueado.";
    [SerializeField] private string unlockedPrompt = "Presiona E para entrar al laboratorio.";
    [SerializeField] private string lockedMessage = "Acceso bloqueado. Completa la verificación primero.";

    private void Awake()
    {
        if (slidingDoor == null)
            slidingDoor = GetComponent<SlidingDoorGroup>();

        if (slidingDoor != null)
            slidingDoor.SetLocked(!unlocked);
    }

    public string GetInteractionPrompt()
    {
        return unlocked ? unlockedPrompt : lockedPrompt;
    }

    public void Interact()
    {
        if (!unlocked)
        {
            FeedbackUI.Message(lockedMessage, 2f);
            return;
        }

        if (slidingDoor != null)
            slidingDoor.Open();
    }

    public void SetUnlocked(bool value)
    {
        unlocked = value;

        if (slidingDoor != null)
            slidingDoor.SetLocked(!value);
    }

    public void UnlockAndOpen()
    {
        SetUnlocked(true);

        if (slidingDoor != null)
            slidingDoor.Open();
    }
}
