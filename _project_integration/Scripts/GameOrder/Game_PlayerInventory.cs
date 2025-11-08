using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem {
    public string itemId;
    public int amount;
}

[System.Serializable]
public class Game_PlayerInventory : MonoBehaviour {
    public List<InventoryItem> items = new List<InventoryItem>();

    public void AddItem(string id, int count) {
        var item = items.Find(i => i.itemId == id);
        if (item != null) item.amount += count;
        else items.Add(new InventoryItem { itemId = id, amount = count });
    }
	
	public int GetAmount(string itemId)
    {
        var it = items.Find(i => i.itemId == itemId);
        return it != null ? it.amount : 0;
    }

    public void RemoveItem(string id, int count) {
        var item = items.Find(i => i.itemId == id);
        if (item != null) {
            item.amount -= count;
            if (item.amount <= 0) items.Remove(item);
        }
    }
}
