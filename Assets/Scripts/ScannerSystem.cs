using System.Collections;
using UnityEngine;

public class ScannerSystem : MonoBehaviour
{
    public static ScannerSystem Instance { get; private set; }

    [Header("Settings")]
    [SerializeField] private float scanDuration = 3f;

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [SerializeField] private float resultDisplayTime = 2f;
    public bool IsShowingResult { get; private set; }

    private Coroutine scanRoutine;
    private IScannable currentTarget;

    private bool isScanning;

    public bool IsScanning => isScanning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void StartScan(IScannable target)
    {
        if (target == null || isScanning || IsShowingResult)
            return;

        currentTarget = target;
        scanRoutine = StartCoroutine(ScanRoutine());
    }

    public void CancelScan()
    {
        if (!isScanning)
            return;

        if (scanRoutine != null)
            StopCoroutine(scanRoutine);

        isScanning = false;

        if (playerController != null)
            playerController.EnableMovement(true);

        currentTarget?.OnScanCancelled();

        currentTarget = null;
    }

    private IEnumerator ScanRoutine()
    {
        isScanning = true;

        IScannable target = currentTarget;

        if (playerController != null)
            playerController.EnableMovement(false);

        target.OnScanStarted();

        if (TabletUI.Instance != null)
            TabletUI.Instance.ShowScanning();

        yield return new WaitForSeconds(scanDuration);

        if (target != null)
        {
            bool safe = target.IsSafe();

            target.OnScanCompleted();

            if (TabletUI.Instance != null)
                TabletUI.Instance.ShowResult(safe);

            IsShowingResult = true;
        }

        if (playerController != null)
            playerController.EnableMovement(true);

        isScanning = false;
        currentTarget = null;

        yield return new WaitForSeconds(resultDisplayTime);

        IsShowingResult = false;
    }
}
