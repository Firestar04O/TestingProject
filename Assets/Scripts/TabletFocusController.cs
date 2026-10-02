using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class TabletFocusController : MonoBehaviour
{
    [Header("Focus Transform")]
    [SerializeField] private Vector3 focusedLocalPosition;
    [SerializeField] private Vector3 focusedLocalEulerRotation;
    [SerializeField] private Vector3 focusedLocalScale = Vector3.one;

    [Header("Transition")]
    [SerializeField] private float moveSpeed = 10f;

    private Vector3 defaultLocalPosition;
    private Quaternion defaultLocalRotation;
    private Vector3 defaultLocalScale;

    private Vector3 targetLocalPosition;
    private Quaternion targetLocalRotation;
    private Vector3 targetLocalScale;

    private bool isFocused;
    private Coroutine moveRoutine;

    private void Awake()
    {
        defaultLocalPosition = transform.localPosition;
        defaultLocalRotation = transform.localRotation;
        defaultLocalScale = transform.localScale;

        targetLocalPosition = defaultLocalPosition;
        targetLocalRotation = defaultLocalRotation;
        targetLocalScale = defaultLocalScale;

        HideTablet();
    }

    public void ShowTablet()
    {
        gameObject.SetActive(true);
        ResetFocus();
    }

    public void HideTablet()
    {
        ResetFocus();
        gameObject.SetActive(false);
    }

    public void OnTabPressed(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!gameObject.activeSelf)
            return;

        ToggleFocus();
    }

    private void ToggleFocus()
    {
        isFocused = !isFocused;

        if (isFocused)
        {
            targetLocalPosition = focusedLocalPosition;
            targetLocalRotation = Quaternion.Euler(focusedLocalEulerRotation);
            targetLocalScale = focusedLocalScale;
        }
        else
        {
            targetLocalPosition = defaultLocalPosition;
            targetLocalRotation = defaultLocalRotation;
            targetLocalScale = defaultLocalScale;
        }

        StartMoveRoutine();
    }

    private void ResetFocus()
    {
        isFocused = false;

        targetLocalPosition = defaultLocalPosition;
        targetLocalRotation = defaultLocalRotation;
        targetLocalScale = defaultLocalScale;

        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        transform.localPosition = defaultLocalPosition;
        transform.localRotation = defaultLocalRotation;
        transform.localScale = defaultLocalScale;
    }

    private void StartMoveRoutine()
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                targetLocalPosition,
                Time.deltaTime * moveSpeed
            );

            transform.localRotation = Quaternion.Lerp(
                transform.localRotation,
                targetLocalRotation,
                Time.deltaTime * moveSpeed
            );

            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetLocalScale,
                Time.deltaTime * moveSpeed
            );

            bool positionReached =
                Vector3.Distance(transform.localPosition, targetLocalPosition) < 0.001f;

            bool rotationReached =
                Quaternion.Angle(transform.localRotation, targetLocalRotation) < 0.1f;

            bool scaleReached =
                Vector3.Distance(transform.localScale, targetLocalScale) < 0.001f;

            if (positionReached && rotationReached && scaleReached)
                break;

            yield return null;
        }

        transform.localPosition = targetLocalPosition;
        transform.localRotation = targetLocalRotation;
        transform.localScale = targetLocalScale;

        moveRoutine = null;
    }
}