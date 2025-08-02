using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public InventoryItem item;

    public void PickupItem()
    {
        Inventory.Instance.AddItem(item);
        Destroy(gameObject);
    }
}
