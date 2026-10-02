using UnityEngine;

public enum CursorModeType
{
    Gameplay,
    UI
}
public class CursorManager : MonoBehaviour
{
    [Header("Start Settings")]
    [SerializeField] private CursorModeType startMode = CursorModeType.Gameplay;
    [SerializeField] private bool applyOnStart = true;

    private CursorModeType currentMode;

    private void Start()
    {
        if (applyOnStart)
            SetMode(startMode);
    }

    public void SetMode(CursorModeType mode)
    {
        currentMode = mode;

        switch (mode)
        {
            case CursorModeType.Gameplay:
                LockCursor();
                break;

            case CursorModeType.UI:
                UnlockCursor();
                break;
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ToggleMode()
    {
        if (currentMode == CursorModeType.Gameplay)
            SetMode(CursorModeType.UI);
        else
            SetMode(CursorModeType.Gameplay);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            UnlockCursor();
        }
        else if (currentMode == CursorModeType.Gameplay)
        {
            LockCursor();
        }
    }

    public CursorModeType CurrentMode => currentMode;
}