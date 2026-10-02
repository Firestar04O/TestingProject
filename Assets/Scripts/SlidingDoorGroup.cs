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

    private bool isOpen;
    private bool isMoving;
    private Coroutine moveRoutine;
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
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveDoor(true));
    }
    public void Close()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveDoor(false));
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
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public bool IsLocked()
    {
        return isLocked;
    }
}