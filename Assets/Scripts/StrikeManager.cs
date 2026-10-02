using UnityEngine;

public class StrikeManager : MonoBehaviour
{
    [Header("Strikes")]
    [SerializeField] private int currentStrikes = 0;
    [SerializeField] private int maxStrikes = 3;

    [Header("References")]
    [SerializeField] private VerificationUI verificationUI;

    public int CurrentStrikes => currentStrikes;
    public int MaxStrikes => maxStrikes;

    public void SetMaxStrikes(int newMax)
    {
        if (newMax <= 0)
            throw new System.ArgumentOutOfRangeException(nameof(newMax), "El máximo de intentos debe ser mayor que cero.");

        maxStrikes = newMax;
        UpdateUI();
    }

    private void Start()
    {
        ResetStrikes();
    }

    public void ResetStrikes()
    {
        currentStrikes = 0;
        UpdateUI();
    }

    public void AddStrike()
    {
        currentStrikes++;
        currentStrikes = Mathf.Clamp(currentStrikes, 0, maxStrikes);

        UpdateUI();
    }

    public bool HasReachedMaxStrikes()
    {
        return currentStrikes >= maxStrikes;
    }

    private void UpdateUI()
    {
        if (verificationUI != null)
            verificationUI.UpdateStrikes(currentStrikes, maxStrikes);
    }
}