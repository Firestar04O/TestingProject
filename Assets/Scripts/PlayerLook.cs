using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private Transform cameraTarget;

    [SerializeField] private float sensitivityX = 2f;
    [SerializeField] private float sensitivityY = 2f;

    [SerializeField] private bool invertY = false;

    [Header("Vertical Limits")]
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;

    [Header("State")]
    [SerializeField] private bool canLook = true;

    private Vector2 lookInput;
    private float pitch = 0f;

    #region INPUT
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }
    #endregion

    private void Update()
    {
        if (!canLook)
            return;
        Look();
    }
    private void Look()
    {
        float mouseX = lookInput.x * sensitivityX;
        transform.Rotate(Vector3.up * mouseX);
        float mouseY = lookInput.y * sensitivityY;
        if (!invertY)
            mouseY *= -1f;
        pitch += mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraTarget.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
    #region PUBLIC
    public void EnableLook(bool value)
    {
        canLook = value;
    }
    public void SetSensitivity(float horizontal, float vertical)
    {
        sensitivityX = horizontal;
        sensitivityY = vertical;
    }
    public void SetInvertY(bool value)
    {
        invertY = value;
    }
    #endregion
}