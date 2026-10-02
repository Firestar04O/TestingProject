using UnityEngine;

public class PanelController : MonoBehaviour
{
    [SerializeField] GameObject currentPanel;

    public void OpenPanel(GameObject panel)
    {
        if (panel == null)
            return;

        if (currentPanel != null)
            currentPanel.SetActive(false);

        panel.SetActive(true);
        currentPanel = panel;
    }

    public void CloseCurrentPanel()
    {
        if (currentPanel != null)
        {
            currentPanel.SetActive(false);
            currentPanel = null;
        }
    }

    public void ClosePanel(GameObject panel)
    {
        if (panel == null)
            return;

        panel.SetActive(false);

        if (currentPanel == panel)
            currentPanel = null;
    }
}
