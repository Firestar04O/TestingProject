using System.Collections;
using UnityEngine;

public class SlidingDoorGroup : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public class DoorPart
    {
        public Transform part;
        public Vector3 localOpenOffset;
        [HideInInspector] public Vector3 closedLocalPosition;
        [HideInInspector] public Vector3 openLocalPosition;
    }
    [Header("Interaction")]
    [SerializeField] private string interactionPrompt = "Presiona E para abrir/cerrar.";
    [SerializeField] private bool canToggle = true;
    [SerializeField] private bool isLocked;
    [SerializeField] private string lockedMessage;

    [Header("Door Parts")]
    [SerializeField] private DoorPart[] doorParts;

    [Header("Movement")]
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Collider")]
    [SerializeField] private Collider[] collidersToDisableWhileOpen;

    [Header("Synchronized Doors")]
    [Tooltip("Other door groups that must open/close together with this one (e.g. a paired double-leaf gate on the opposite wall).")]
    [SerializeField] private SlidingDoorGroup[] linkedGroups;

    private bool isOpen;
    private bool isMoving;
    private Coroutine moveRoutine;

    public bool IsOpen => isOpen;
    private void Awake()
    {
        foreach (DoorPart doorPart in doorParts)
        {
            if (doorPart.part == null)
                continue;

            doorPart.closedLocalPosition = doorPart.part.localPosition;
            doorPart.openLocalPosition = doorPart.closedLocalPosition + doorPart.localOpenOffset;
        }
    }
    public string GetInteractionPrompt()
    {
        if (isLocked)
            return lockedMessage;

        return interactionPrompt;
    }
    public void Interact()
    {
        if (isLocked)
        {
            FeedbackUI.Message(lockedMessage, 2f);
            return;
        }
        if (isMoving)
            return;

        if (isOpen && !canToggle)
            return;

        if (isOpen)
            Close();
        else
            Open();
    }
    public void Open()
    {
        Open(true);
    }
    public void Close()
    {
        Close(true);
    }
    private void Open(bool propagateToLinked)
    {
        if (isLocked)
            return;

        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveDoor(true));

        if (propagateToLinked)
            SyncLinkedGroups(true);
    }
    private void Close(bool propagateToLinked)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveDoor(false));

        if (propagateToLinked)
            SyncLinkedGroups(false);
    }
    // Tells every door group wired into linkedGroups to open/close alongside this one.
    // The "propagateToLinked: false" call on the linked group prevents it from calling
    // back into this group (and anything further down the chain), so two gates that
    // link to each other can't loop forever.
    private void SyncLinkedGroups(bool open)
    {
        Debug.Log($"[SlidingDoorGroup] {gameObject.name} SyncLinkedGroups(open={open}), linkedGroups count={(linkedGroups == null ? 0 : linkedGroups.Length)}");

        if (linkedGroups == null)
            return;

        foreach (SlidingDoorGroup linked in linkedGroups)
        {
            if (linked == null || linked == this)
            {
                continue;
            }

            Debug.Log($"[SlidingDoorGroup] {gameObject.name} -> propagating to {linked.gameObject.name}");

            if (open)
                linked.Open(false);
            else
                linked.Close(false);
        }
    }
    private IEnumerator MoveDoor(bool open)
    {
        isMoving = true;

        SetCollidersEnabled(false);

        float elapsed = 0f;

        Vector3[] startPositions = new Vector3[doorParts.Length];
        Vector3[] targetPositions = new Vector3[doorParts.Length];

        for (int i = 0; i < doorParts.Length; i++)
        {
            startPositions[i] = doorParts[i].part.localPosition;
            targetPositions[i] = open ? doorParts[i].openLocalPosition : doorParts[i].closedLocalPosition;
        }

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / moveDuration;
            float curvedT = movementCurve.Evaluate(t);

            for (int i = 0; i < doorParts.Length; i++)
            {
                doorParts[i].part.localPosition = Vector3.Lerp(
                    startPositions[i],
                    targetPositions[i],
                    curvedT
                );
            }

            yield return null;
        }

        for (int i = 0; i < doorParts.Length; i++)
        {
            doorParts[i].part.localPosition = targetPositions[i];
        }

        isOpen = open;
        isMoving = false;

        SetCollidersEnabled(!isOpen);
    }
    private void SetCollidersEnabled(bool value)
    {
        foreach (Collider col in collidersToDisableWhileOpen)
        {
            if (col != null)
                col.enabled = value;
        }
    }
    public void SetLocked(bool value)
    {
        isLocked = value;

        if (linkedGroups == null)
            return;

        // Linked gates share a lock. Apply it once per link without calling back.
        foreach (SlidingDoorGroup linked in linkedGroups)
        {
            if (linked != null)
                linked.isLocked = value;
        }
    }

    public void Lock()
    {
        SetLocked(true);
    }

    public void Unlock()
    {
        SetLocked(false);
    }

    public bool IsLocked()
    {
        return isLocked;
    }
}
