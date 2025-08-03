using System;
using UnityEngine;

public class SwingInteractable : MonoBehaviour, IInteractable
{
    public Transform swingArm;             // The part that rotates
    public float maxSwingAngle = 90f;      // Max angle in degrees
    public float swingSpeed = 2f;          // How fast it swings
    public float pushIncrement = 5f;       // How much to add per push
    public float dampingSpeed = 1f;        // How fast it slows down

    public float currentAmplitude = 0f;   // Current swing amplitude
    private float time = 0f;
    private bool isSwinging = false;

    [SerializeField] SphereCollider swingCollider;

    void Start()
    {
        KittyController.instance.swingArm = swingArm;
    }

    void Update()
    {
        if (!isSwinging || swingArm == null) return;

        time += Time.deltaTime * swingSpeed;

        // Slowly reduce amplitude
        currentAmplitude = Mathf.MoveTowards(currentAmplitude, 0f, dampingSpeed * Time.deltaTime);

        float angle = Mathf.Sin(time) * currentAmplitude;
        Vector3 currentEuler = swingArm.localEulerAngles;
        swingArm.localRotation = Quaternion.Euler(angle, currentEuler.y, currentEuler.z);

        // Stop swinging when amplitude is very small
        if (Mathf.Abs(currentAmplitude) < 0.1f)
        {
            isSwinging = false;
            currentAmplitude = 0f;
        }
    }

    public void Interact(GameObject caller)
    {
        isSwinging = true;

        // Increase amplitude, capped at maxSwingAngle
        currentAmplitude += pushIncrement;
        currentAmplitude = Mathf.Clamp(currentAmplitude, 0f, maxSwingAngle);
    }
}
