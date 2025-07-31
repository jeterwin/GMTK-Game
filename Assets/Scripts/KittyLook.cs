using UnityEngine;

public class KittyLook : MonoBehaviour
{
    public Transform playerBody;        // Assign the KittyPlayer (root)
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Look up/down
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -60f, 60f); // Limit up/down angle

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotate the body left/right
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
