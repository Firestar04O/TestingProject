using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("States")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private bool canInteract = true;
    [SerializeField] private bool canScan = true;
    [SerializeField] private bool invertControls = false;

    private CharacterController controller;
    private Vector2 moveInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
        MovePlayer();
    }
    #region INPUT
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (invertControls)
            moveInput *= -1f;
    }
    #endregion

    #region MOVEMENT
    private void MovePlayer()
    {
        if (!canMove)
            return;

        Vector3 direction = transform.forward * moveInput.y + transform.right * moveInput.x;

        if (direction.sqrMagnitude > 1f)
            direction.Normalize();

        controller.Move(direction * moveSpeed * Time.deltaTime);
    }
    #endregion

    #region PUBLIC FUNCTIONS
    public void EnableMovement(bool value)
    {
        canMove = value;
    }
    public void EnableInteraction(bool value)
    {
        canInteract = value;
    }
    public void EnableScanner(bool value)
    {
        canScan = value;
    }
    public void SetInvertControls(bool value)
    {
        invertControls = value;
    }
    public void SetMoveSpeed(float value)
    {
        moveSpeed = value;
    }
    #endregion

    #region GETTERS
    public bool CanMove => canMove;
    public bool CanInteract => canInteract;
    public bool CanScan => canScan;
    #endregion
}
