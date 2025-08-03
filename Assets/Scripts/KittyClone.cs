using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KittyClone : MonoBehaviour
{
    private List<PlayerFrameData> playbackData;
    private List<InventoryItem> inventory = new List<InventoryItem>();

    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

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
        for (int i = 0; i < playbackData.Count;)
        {
            if (Time.timeScale != 0)
            {
                var frame = playbackData[i];

                transform.position = frame.position;
                transform.rotation = frame.rotation;

                PlayAnimation(frame.animationState);

                if (frame.interactionPressed)
                {
                    TriggerInteraction();
                }

                i++; // Advance to next frame only after processing
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
