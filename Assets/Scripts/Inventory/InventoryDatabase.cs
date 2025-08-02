using UnityEngine;
using System.Collections.Generic;

public class InventoryDatabase : MonoBehaviour
{
    public static InventoryDatabase Instance;
    public List<InventoryItem> allItems;

    private Dictionary<string, InventoryItem> itemDict;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        itemDict = new Dictionary<string, InventoryItem>();
        foreach (var item in allItems)
        {
            itemDict[item.itemName] = item;
        }
    }

    public InventoryItem GetItemByName(string name)
    {
        itemDict.TryGetValue(name, out var item);
        return item;
    }
}
