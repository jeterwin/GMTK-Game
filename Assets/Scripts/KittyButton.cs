using System;
using UnityEngine;
using UnityEngine.Events;

public class KittyButton : MonoBehaviour, IInteractable
{
    [SerializeField] string requiredToolName;

    public UnityEvent OnInteract;

    private GameObject currentKittyInRange;

    public void Interact(GameObject caller)
    {
        if (!KittyController.instance.isOnSwing)
        {
            if (requiredToolName == null || requiredToolName == "" || (Inventory.Instance.HasItem(requiredToolName) && caller.CompareTag("Player")))
            {
                Debug.Log($"{caller.name} interacted with the button!");
                OnInteract?.Invoke();
            }
        }
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
        DialogueManager.Instance.kittenMeow.Play(); // Play the kitten meow sound
        LevelManager.Instance.KittenFound();
        Destroy(gameObject); // Destroy the button after interaction
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
