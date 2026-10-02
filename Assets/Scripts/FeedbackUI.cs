using System.Collections;
using TMPro;
using UnityEngine;

public class FeedbackUI : MonoBehaviour
{
    public static FeedbackUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject messageGroup;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Optional dedicated prompt line")]
    [SerializeField] private GameObject promptGroup;
    [SerializeField] private TextMeshProUGUI promptText;

    [Header("Settings")]
    [SerializeField] private float defaultDuration = 2f;
    [SerializeField] private float promptDuration = 1.5f;

    private Coroutine currentRoutine;
    private bool criticalMessageActive;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        HideMessageInstant();
        HidePrompt();
    }

    public static void Message(string message)
    {
        if (Instance != null)
            Instance.ShowMessage(message);
    }

    public static void Message(string message, float duration)
    {
        if (Instance != null)
            Instance.ShowMessage(message, duration);
    }

    public void ShowMessage(string message)
    {
        ShowMessage(message, defaultDuration);
    }

    public void ShowMessage(string message, float duration)
    {
        if (this == null) // instancia destruida: Unity compara como null con su == sobrecargado
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        criticalMessageActive = true;
        currentRoutine = StartCoroutine(ShowMessageRoutine(message, duration));
    }

    public void ShowPrompt(string prompt)
    {
        if (this == null)
            return;

        if (promptText != null)
        {
            promptText.text = prompt ?? "";
            if (promptGroup != null)
                promptGroup.SetActive(!string.IsNullOrEmpty(prompt));

            return;
        }

        if (criticalMessageActive)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessageRoutine(prompt, promptDuration));
    }

    public void HidePrompt()
    {
        if (this == null)
            return;

        if (promptText != null)
            promptText.text = "";

        if (promptGroup != null)
            promptGroup.SetActive(false);
    }

    private IEnumerator ShowMessageRoutine(string message, float duration)
    {
        if (messageGroup != null)
            messageGroup.SetActive(true);

        if (messageText != null)
            messageText.text = message;

        yield return new WaitForSeconds(duration);

        HideMessageInstant();
    }

    public void HideMessageInstant()
    {
        criticalMessageActive = false;

        if (messageText != null)
            messageText.text = "";

        if (messageGroup != null)
            messageGroup.SetActive(false);
    }
}
