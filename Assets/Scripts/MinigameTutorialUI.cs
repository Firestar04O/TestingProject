using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameTutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text instructionsText;
    [SerializeField] private Button continueButton;

    private Action onContinue;

    private void Awake()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(Continue);
            continueButton.onClick.AddListener(Continue);
        }

        Hide();
    }

    public void Show(Action continuation)
    {
        onContinue = continuation;

        if (titleText != null)
            titleText.text = "Tutorial de verificación";

        if (instructionsText != null)
        {
            instructionsText.text =
                "1. Observa las tres puertas y revisa la evidencia mostrada.\n" +
                "2. Apunta con la mira y presiona E para elegir la opción más segura.\n" +
                "3. Completa los cuatro niveles antes de que termine el tiempo.\n" +
                "4. Una decisión incorrecta reduce la integridad del intento.";
        }

        if (panel != null)
            panel.SetActive(true);

        if (continueButton != null)
            continueButton.Select();
    }

    public void Continue()
    {
        Action continuation = onContinue;
        onContinue = null;
        Hide();
        continuation?.Invoke();
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
