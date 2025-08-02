using UnityEngine;

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

    [Header("Run Settings")]
    public float runMultiplier = 1.8f;
    public KeyCode runKey = KeyCode.LeftShift;

    private bool isRunning = false;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;

    [Header("Camera Settings")]
    public Transform cameraPivot;  // For pitch and tilt
    public Transform kittyCamera;  // Actual camera
    public float swayAmount = 0.05f;
    public float normalSwaySpeed = 4f;
    public float runSwaySpeed = 6f;
    public Transform cameraRoot;  // Parent transform for yaw rotation & position
    public Vector3 cameraOffset;

    [Header("Advanced Movement")]
    public float airControlFactor = 0.3f;
    public float maxHorizontalSpeed = 10f;
    public float airDrag = 0.1f;

    [Header("Camera Tilt & Bounce")]
    public float strafeTiltAmount = 5f;
    public float tiltSmoothSpeed = 5f;
    public float landingBounceAmount = 0.08f;
    public float landingBounceSpeed = 6f;

    private float currentTilt = 0f;
    private float tiltVelocity = 0f;
    private float bounceOffset = 0f;
    private float bounceVelocity = 0f;
    private bool wasGrounded = false;

    private Rigidbody rb;
    private Vector3 input;
    private Vector3 originalCamLocalPos;
    private float pitch = 0f;
    private float yaw = 0f;
    private float swaySpeed;

    private BoxCollider box;

    private Vector2 smoothMouseDelta;
    private Vector2 currentMouseVelocity;
    public float smoothTime = 0.05f;  // tweak this value to your liking

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        swaySpeed = normalSwaySpeed;
        box = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        originalCamLocalPos = kittyCamera.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Position cameraRoot at character position + offset
        cameraRoot.position = transform.position
            + transform.forward * cameraOffset.z
            + transform.right * cameraOffset.x
            + transform.up * cameraOffset.y;

        HandleMouseLook();
        HandleInput();
        UpdateTiltAndBounce();
        UpdateCameraPosition();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleInput()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        isRunning = Input.GetKey(runKey);

        if (isRunning)
        {
            swaySpeed = runSwaySpeed;
        }
        else
        {
            swaySpeed = normalSwaySpeed;
        }

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Move()
    {
        Vector3 moveDir = transform.forward * input.z + transform.right * input.x;
        float currentSpeed = isRunning ? moveSpeed * runMultiplier : moveSpeed;
        Vector3 targetVelocity = moveDir * currentSpeed;


        Vector3 currentVelocity = rb.linearVelocity;

        Vector3 velocityChange = new Vector3(
            targetVelocity.x - currentVelocity.x,
            0f,
            targetVelocity.z - currentVelocity.z
        );

        float control = IsGrounded() ? 1f : airControlFactor;

        rb.AddForce(velocityChange * acceleration * control, ForceMode.Acceleration);

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizontalVelocity.magnitude > maxHorizontalSpeed)
        {
            Vector3 limitedVelocity = horizontalVelocity.normalized * maxHorizontalSpeed;
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z);
        }

        if (!IsGrounded())
        {
            rb.linearVelocity *= (1f - airDrag * Time.fixedDeltaTime);
        }
    }

    void HandleMouseLook()
    {
        // Get raw mouse delta (scaled by sensitivity, NOT by Time.deltaTime here)
        Vector2 targetMouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;

        // Smooth the mouse delta over time
        smoothMouseDelta = Vector2.SmoothDamp(smoothMouseDelta, targetMouseDelta, ref currentMouseVelocity, smoothTime);

        // Apply smoothed mouse delta scaled by Time.deltaTime
        float mouseX = smoothMouseDelta.x * Time.deltaTime;
        float mouseY = smoothMouseDelta.y * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        float bodyYaw = NormalizeAngle(transform.eulerAngles.y);
        yaw = ClampAngle(yaw, bodyYaw - 100f, bodyYaw + 100f);

        cameraRoot.rotation = Quaternion.Euler(0f, yaw, 0f);
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        if (input.magnitude > 0.01f)
        {
            Quaternion targetBodyRotation = Quaternion.Euler(0f, yaw, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetBodyRotation, Time.deltaTime * 10f);
        }
    }


    // Helper to clamp angles correctly, handling wrap-around
    float ClampAngle(float angle, float min, float max)
    {
        angle = NormalizeAngle(angle);
        min = NormalizeAngle(min);
        max = NormalizeAngle(max);

        bool inverse = false;
        float tMin = min;
        float tMax = max;

        if (min > max)
        {
            inverse = true;
        }

        if (!inverse)
        {
            if (angle < min) return min;
            if (angle > max) return max;
            return angle;
        }
        else
        {
            if (angle > max && angle < min)
            {
                // Outside allowed range, clamp to nearest bound
                float distToMin = Mathf.Abs(angle - min);
                float distToMax = Mathf.Abs(angle - max);
                return (distToMin < distToMax) ? min : max;
            }
            return angle;
        }
    }

    // Normalize angle between -180 and 180
    float NormalizeAngle(float angle)
    {
        while (angle > 180f) angle -= 360f;
        while (angle < -180f) angle += 360f;
        return angle;
    }



    void UpdateTiltAndBounce()
    {
        // Tilt cameraPivot based on strafing
        float targetTilt = -input.x * strafeTiltAmount;
        currentTilt = Mathf.SmoothDamp(currentTilt, targetTilt, ref tiltVelocity, 1f / tiltSmoothSpeed);

        // Bounce on landing
        bool grounded = IsGrounded();
        if (!wasGrounded && grounded)
        {
            bounceVelocity = -landingBounceAmount;
        }

        bounceOffset = Mathf.SmoothDamp(bounceOffset, 0f, ref bounceVelocity, 1f / landingBounceSpeed);
        wasGrounded = grounded;

        // Combine pitch and tilt rotation on cameraPivot
        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, currentTilt);
    }

    void UpdateCameraPosition()
    {
        // Calculate sway offsets
        float sway = Mathf.Sin(Time.time * swaySpeed) * swayAmount * input.magnitude;
        float bob = Mathf.Abs(Mathf.Cos(Time.time * swaySpeed * 2)) * swayAmount * 0.3f * input.magnitude;
        Vector3 swayOffset = new Vector3(sway, bob, 0f);

        // Combine sway and bounce offsets for final camera local position
        Vector3 finalOffset = originalCamLocalPos + swayOffset + new Vector3(0f, bounceOffset, 0f);

        // Smoothly interpolate to final position
        kittyCamera.localPosition = Vector3.Lerp(kittyCamera.localPosition, finalOffset, Time.deltaTime * 8f);
    }

    bool IsGrounded()
    {
        if (!box) return false;

        Vector3 boxCenterWorld = transform.position + box.center - new Vector3(0, box.size.y / 2f + 0.05f, 0);
        Vector3 halfExtents = new Vector3(box.size.x * 0.5f * 0.95f, 0.05f, box.size.z * 0.5f * 0.95f);

        return Physics.CheckBox(boxCenterWorld, halfExtents, transform.rotation, groundLayer, QueryTriggerInteraction.Ignore);
    }

}
