using UnityEngine;

public class TimerSystem : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float startTime = 30f;
    [SerializeField] private float currentTime;
    [SerializeField] private bool timerRunning;

    [Header("References")]
    [SerializeField] private VerificationManager verificationManager;
    [SerializeField] private VerificationUI verificationUI;

    public float CurrentTime => currentTime;
    public bool TimerRunning => timerRunning;

    private void Update()
    {
        if (!timerRunning)
            return;

        TickTimer();
    }

    public void StartTimer()
    {
        currentTime = startTime;
        timerRunning = true;

        UpdateUI();
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public void ResetTimer()
    {
        currentTime = startTime;
        UpdateUI();
    }

    private void TickTimer()
    {
        currentTime -= Time.deltaTime;
        currentTime = Mathf.Clamp(currentTime, 0f, startTime);

        UpdateUI();

        if (currentTime <= 0f)
            HandleTimeExpired();
    }

    private void HandleTimeExpired()
    {
        timerRunning = false;

        if (verificationManager != null)
            verificationManager.TimeExpired();
    }

    private void UpdateUI()
    {
        if (verificationUI != null)
            verificationUI.UpdateTimer(currentTime);
    }
}