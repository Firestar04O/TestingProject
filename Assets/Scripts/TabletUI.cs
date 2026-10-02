using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabletUI : MonoBehaviour
{
    public static TabletUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Image evidenceImage;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Default")]
    [SerializeField] private string defaultText = "Sistema de verificación listo.";

    private void Awake()
    {
        Instance = this;
        ClearTablet();
    }

    public void ShowEvidence(Sprite sprite)
    {
        if (evidenceImage != null)
        {
            evidenceImage.enabled = sprite != null;
            evidenceImage.sprite = sprite;
        }

        if (statusText != null)
            statusText.text = "Evidencia detectada.";
    }

    public void ShowScanning()
    {
        if (statusText != null)
            statusText.text = "Escaneando...";
    }

    public void ShowResult(bool safe)
    {
        if (statusText != null)
            statusText.text = safe ? "Resultado: SEGURO" : "Resultado: INSEGURO";
    }

    public void ClearTablet()
    {
        if (evidenceImage != null)
        {
            evidenceImage.sprite = null;
            evidenceImage.enabled = false;
        }

        if (statusText != null)
            statusText.text = defaultText;
    }
    public void ShowEvidenceImageOnly(Sprite sprite)
    {
        if (evidenceImage != null)
        {
            evidenceImage.enabled = sprite != null;
            evidenceImage.sprite = sprite;
        }
    }
}