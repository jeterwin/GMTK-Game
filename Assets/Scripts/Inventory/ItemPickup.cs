using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory.Instance.AddItem(item);
            Destroy(gameObject);
        }
    }
}
