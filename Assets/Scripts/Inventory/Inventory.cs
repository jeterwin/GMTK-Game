using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    public List<InventoryItem> items = new List<InventoryItem>();
    public int maxSlots = 10;

    public delegate void OnInventoryChanged();
    public event OnInventoryChanged onInventoryChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public bool AddItem(InventoryItem item)
    {
        if (items.Count >= maxSlots) return false;
        items.Add(item);
        onInventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.itemName == itemName);
    }
}
