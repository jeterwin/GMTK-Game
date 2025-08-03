using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittyClone : MonoBehaviour
{
    private List<PlayerFrameData> playbackData;
    private List<InventoryItem> inventory = new List<InventoryItem>();

    public void Init(List<PlayerFrameData> segment)
    {
        try
        {        
            playbackData = segment;
            SetCloneInventory(segment[segment.Count - 1].inventorySnapshot); // Snapshot at rewind
            StartCoroutine(PlayReplay());
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Clone Init failed: " + ex.Message);
        }
    }

    IEnumerator PlayReplay()
    {
        foreach (var frame in playbackData)
        {
            transform.position = frame.position;
            transform.rotation = frame.rotation;

            // Play animation if needed
            PlayAnimation(frame.animationState);

            // Trigger interaction if this frame recorded a press
            if (frame.interactionPressed)
            {
                TriggerInteraction(); // Define this method to call nearby IInteractables
            }

            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }

    void PlayAnimation(string animState) {
        // Your Animator handling
    }

    void TriggerInteraction()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hit in hits)
        {
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact(gameObject); // Pass the clone as the caller
            }
        }
    }

    void SetCloneInventory(List<string> itemNames)
    {
        if (itemNames == null) return;

        inventory = new List<InventoryItem>();

        foreach (var name in itemNames)
        {
            var item = InventoryDatabase.Instance?.GetItemByName(name);
            if (item != null)
                inventory.Add(item);
            else
                Debug.LogWarning($"Item not found in database: {name}");
        }
    }
}
