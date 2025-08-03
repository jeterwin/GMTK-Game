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
        if (other.CompareTag("Player") || other.CompareTag("Clone")) // Make sure the kitty has the "Player" tag
        {
            currentKittyInRange = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Player") || other.CompareTag("Clone")) && other.gameObject == currentKittyInRange)
        {
            currentKittyInRange = null;
        }
    }

    public void InteractedWithKitten()
    {
        LevelManager.Instance.KittenFound();
        Destroy(gameObject); // Destroy the button after interaction
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
