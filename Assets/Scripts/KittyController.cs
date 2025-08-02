using UnityEngine;
using static UnityEngine.UI.Image;

[RequireComponent(typeof(Rigidbody))]
public class KittyController : MonoBehaviour
{
    public static KittyController instance;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float acceleration = 10f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.3f;
    public LayerMask groundLayer;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;

    [Header("Camera Settings")]
    public Transform cameraPivot;  // For sway
    public Transform kittyCamera;  // Actual camera
    public float swayAmount = 0.05f;
    public float swaySpeed = 4f;

    private Rigidbody rb;
    private Vector3 input;
    private Vector3 originalCamLocalPos;
    private float pitch = 0f;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalCamLocalPos = kittyCamera.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleInput();
        ApplyCameraSway();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleInput()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Move()
    {
        // Move in the forward/right direction of the body
        Vector3 moveDir = transform.forward * input.z + transform.right * input.x;
        Vector3 targetVelocity = moveDir * moveSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = new Vector3(
            targetVelocity.x - currentVelocity.x,
            0f,
            targetVelocity.z - currentVelocity.z
        );

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotate body (yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Tilt camera (pitch)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -60f, 60f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void ApplyCameraSway()
    {
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount * input.magnitude;
        float bob = Mathf.Abs(Mathf.Cos(Time.time * swaySpeed * 2)) * swayAmount * 0.5f * input.magnitude;

        Vector3 swayOffset = new Vector3(sway, bob, 0f);
        kittyCamera.localPosition = Vector3.Lerp(kittyCamera.localPosition, originalCamLocalPos + swayOffset, Time.deltaTime * 8f);
    }

    bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}
