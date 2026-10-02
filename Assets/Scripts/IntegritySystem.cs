using UnityEngine;

public class IntegritySystem : MonoBehaviour
{
    [Header("Integrity")]
    [SerializeField] private float maxIntegrity = 100f;
    [SerializeField] private float currentIntegrity = 100f;
    [SerializeField] private float damagePerMistake = 25f;

    [Header("References")]
    [SerializeField] private VerificationManager verificationManager;
    [SerializeField] private VerificationUI verificationUI;

    public float CurrentIntegrity => currentIntegrity;
    public float MaxIntegrity => maxIntegrity;

    public void ResetIntegrity()
    {
        currentIntegrity = maxIntegrity;
        UpdateUI();
    }

    public void TakeDamage()
    {
        currentIntegrity -= damagePerMistake;
        currentIntegrity = Mathf.Clamp(currentIntegrity, 0f, maxIntegrity);

        UpdateUI();

        if (currentIntegrity <= 0f)
            HandleIntegrityDepleted();
    }

    private void HandleIntegrityDepleted()
    {
        if (verificationManager != null)
            verificationManager.FailVerification();
    }

    private void UpdateUI()
    {
        if (verificationUI != null)
            verificationUI.UpdateIntegrity(currentIntegrity, maxIntegrity);
    }
}