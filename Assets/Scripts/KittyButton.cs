using UnityEngine;
using UnityEngine.Events;

public class KittyButton : MonoBehaviour, IInteractable
{
    public UnityEvent OnInteract;

    private GameObject currentKittyInRange;

    public void Interact(GameObject caller)
    {
        Debug.Log($"{caller.name} interacted with the button!");
        OnInteract?.Invoke();
    }

    void Update()
    {
        if (currentKittyInRange != null && Input.GetKeyDown(KeyCode.E))
        {
            Interact(currentKittyInRange);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the kitty has the "Player" tag
        {
            currentKittyInRange = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == currentKittyInRange)
        {
            currentKittyInRange = null;
        }
    }
}
