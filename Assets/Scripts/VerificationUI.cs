using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VerificationUI : MonoBehaviour
{
    [Header("HUD Groups")]
    [SerializeField] private GameObject hudCommon;
    [SerializeField] private GameObject hudExploration;
    [SerializeField] private GameObject hudVerification;
    [SerializeField] private GameObject hudPause;

    [Header("Common UI")]
    [SerializeField] private TMP_Text strikesText;

    [Header("Verification UI")]
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text integrityText;
    [SerializeField] private Slider integritySlider;

    public void ShowExplorationUI()
    {
        if (hudCommon != null)
            hudCommon.SetActive(true);

        if (hudExploration != null)
            hudExploration.SetActive(true);

        if (hudVerification != null)
            hudVerification.SetActive(false);

        if (hudPause != null)
            hudPause.SetActive(false);
    }

    public void ShowVerificationUI()
    {
        if (hudCommon != null)
            hudCommon.SetActive(true);

        if (hudExploration != null)
            hudExploration.SetActive(false);

        if (hudVerification != null)
            hudVerification.SetActive(true);

        if (hudPause != null)
            hudPause.SetActive(false);
    }

    public void ShowPauseUI()
    {
        if (hudPause != null)
            hudPause.SetActive(true);
    }

    public void HidePauseUI()
    {
        if (hudPause != null)
            hudPause.SetActive(false);
    }

    public void UpdateLevel(int currentLevel)
    {
        if (levelText != null)
            levelText.text = $"Nivel {currentLevel}";
    }

    public void UpdateTimer(float currentTime)
    {
        if (timerText != null)
            timerText.text = $"Tiempo: {Mathf.CeilToInt(currentTime)}";
    }

    public void UpdateIntegrity(float currentIntegrity, float maxIntegrity)
    {
        if (integrityText != null)
            integrityText.text = $"Integridad: {currentIntegrity}/{maxIntegrity}";

        if (integritySlider != null)
            integritySlider.value = currentIntegrity / maxIntegrity;
    }

    public void UpdateStrikes(int currentStrikes, int maxStrikes)
    {
        if (strikesText != null)
            strikesText.text = $"Strikes: {currentStrikes}/{maxStrikes}";
    }
}