using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayers;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerController playerController;

    private IInteractable currentInteractable;
    private IScannable currentScannable;

    private IInteractable lastInteractable;
    private readonly InteractionTargetCache targetCache = new InteractionTargetCache();

    private void Awake()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerController == null)
            playerController = FindFirstObjectByType<PlayerController>();
    }

    private void Update()
    {
        CheckForInteractable();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (playerController != null && !playerController.CanInteract)
            return;

        if (currentInteractable == null)
        {
            FeedbackUI.Message("No hay nada para interactuar.");
            return;
        }

        currentInteractable.Interact();
    }

    public void OnScan(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (playerController != null && !playerController.CanScan)
                return;

            if (currentScannable == null)
            {
                FeedbackUI.Message("No hay nada para escanear.");
                return;
            }

            if (ScannerSystem.Instance == null)
            {
                FeedbackUI.Message("El escáner no está disponible.");
                return;
            }

            ScannerSystem.Instance.StartScan(currentScannable);
        }

        if (context.canceled && ScannerSystem.Instance != null)
        {
            ScannerSystem.Instance.CancelScan();
        }
    }

    private void CheckForInteractable()
    {
        if (ScannerSystem.Instance != null && ScannerSystem.Instance.IsScanning)
            return;

        Collider detectedCollider = null;

        if (playerCamera != null)
        {
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayers))
                detectedCollider = hit.collider;
        }

        targetCache.Refresh(detectedCollider);
        currentInteractable = targetCache.Interactable;
        currentScannable = targetCache.Scannable;

        if (currentInteractable == lastInteractable)
            return;

        lastInteractable = currentInteractable;

        bool canUpdateTablet = ScannerSystem.Instance == null || !ScannerSystem.Instance.IsShowingResult;

        if (currentInteractable == null)
        {
            if (FeedbackUI.Instance != null)
                FeedbackUI.Instance.HidePrompt();

            if (canUpdateTablet && TabletUI.Instance != null)
                TabletUI.Instance.ClearTablet();

            return;
        }

        if (FeedbackUI.Instance != null)
            FeedbackUI.Instance.ShowPrompt(currentInteractable.GetInteractionPrompt());

        if (currentScannable is DoorOption door)
        {
            if (TabletUI.Instance != null)
                TabletUI.Instance.ShowEvidenceImageOnly(door.GetEvidenceSprite());
        }
        else if (canUpdateTablet && TabletUI.Instance != null)
        {
            TabletUI.Instance.ClearTablet();
        }
    }

    private void OnDrawGizmos()
    {
        if (playerCamera == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward * interactDistance
        );
    }
}
