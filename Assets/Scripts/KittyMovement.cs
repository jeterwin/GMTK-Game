using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KittyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float acceleration = 10f;
    public float maxSpeed = 5f;
    public float jumpForce = 6f;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;

    [Header("Camera Reference")]
    public Transform cameraTransform; // ? this should be the CameraPivot

    [Header("Camera Sway")]
    public Transform cameraObject; // ? actual Camera (KittyCamera)
    public float swayAmount = 0.05f;
    public float swaySpeed = 4f;

    private Rigidbody rb;
    private Vector3 inputDirection;
    private Vector3 moveVelocity;
    private Vector3 smoothVelocity;
    private Vector3 originalCamLocalPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalCamLocalPos = cameraObject.localPosition;
    }

    void Update()
    {
        HandleInput();
        ApplyCameraSway();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleInput()
    {
        Vector3 rawInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        inputDirection = rawInput.normalized;

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void HandleMovement()
    {
        // Move relative to camera's facing direction
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = camForward * inputDirection.z + camRight * inputDirection.x;
        Vector3 targetVelocity = moveDir * maxSpeed;

        Vector3 velocityChange = Vector3.SmoothDamp(
            new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z),
            targetVelocity,
            ref smoothVelocity,
            0.1f
        );

        rb.linearVelocity = new Vector3(velocityChange.x, rb.linearVelocity.y, velocityChange.z);
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    void ApplyCameraSway()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount * inputDirection.magnitude;
        float bob = Mathf.Abs(Mathf.Cos(Time.time * swaySpeed * 2)) * swayAmount * 0.5f * inputDirection.magnitude;

        Vector3 swayOffset = new Vector3(sway, bob, 0f);
        cameraObject.localPosition = Vector3.Lerp(cameraObject.localPosition, originalCamLocalPos + swayOffset, Time.deltaTime * 8f);
    }
}
