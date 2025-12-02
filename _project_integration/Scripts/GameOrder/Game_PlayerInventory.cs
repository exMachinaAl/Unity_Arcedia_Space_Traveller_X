using UnityEngine;
using System.Collections.Generic;


public enum ItemToolType
{
    None,
    Axe,
    Pickaxe,
    Shovel,
    Hammer
}
public enum ItemType
{
    Material,
    Tool,
    Fuel,
    Consumable,
    Quest,
    Equipment,
    Placeable
}
public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
[System.Serializable]
public class ItemData
{
    public string id;          // UNIQUE, contoh: "iron_ore"
    public string itemName;    // Nama UI
    public string description;

    public ItemType itemType;  // Material, Tool, Fuel, dll
    public ItemToolType toolType; // Jika itemType = Tool
    public int maxStack = 64;

    // --- OPTIONAL BY TYPE ---
    public int durability;     // Untuk Tool
    public float efficiency;  // Mining speed / power

    public float fuelValue;   // Untuk Fuel
    public float healValue;   // Untuk Consumable

    public bool isPlaceable;
    public Sprite icon;
    public GameObject placePrefab;

    public ItemRarity rarity;
}
[System.Serializable]
public class ItemStack
{
    public ItemData item;
    public int amount;
    public int currentDurability; // hanya dipakai kalau tool

    public ItemStack(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;

        if (item.itemType == ItemType.Tool)
            currentDurability = item.durability;
    }

    public bool IsFull()
    {
        return amount >= item.maxStack;
    }
}

public class Game_PlayerInventory : MonoBehaviour
{
    public List<ItemStack> items = new List<ItemStack>();
    public int selectedIndex = 0;

    void Update()
    {
        // Detect tombol angka 1 dan 2
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedIndex = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2)) selectedIndex = 1;

        // Scroll mouse untuk memilih item
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) // Scroll ke atas
        {
            selectedIndex--;
        }
        else if (scroll < 0f) // Scroll ke bawah
        {
            selectedIndex++;
        }

        // Pastikan selectedIndex berada dalam rentang indeks yang valid
        selectedIndex = Mathf.Clamp(selectedIndex, 0, items.Count - 1);

        if (items.Count > 0)
            Manager_UI.Instance.OnChangeOrUpdateInventory(this, selectedIndex);
    }



    // =========================
    // ADD ITEM
    // =========================
    public void AddItem(ItemData newItem, int amount)
    {
        foreach (var stack in items)
        {
            if (stack.item.id == newItem.id && !stack.IsFull())
            {
                int space = newItem.maxStack - stack.amount;
                int add = Mathf.Min(space, amount);

                stack.amount += add;
                amount -= add;

                if (amount <= 0)
                    return;
            }
        }

        // Jika masih sisa → buat stack baru
        ItemStack newStack = new ItemStack(newItem, amount);
        items.Add(newStack);

        //QuestManager + ui
        Manager_Quest.Instance.CheckInventoryItemQuest();
    }

    // =========================
    // REMOVE ITEM
    // =========================
    public bool RemoveItem(string itemId, int amount)
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i].item.id == itemId)
            {
                int take = Mathf.Min(items[i].amount, amount);
                items[i].amount -= take;
                amount -= take;

                if (items[i].amount <= 0)
                    items.RemoveAt(i);

                if (amount <= 0)
                    return true;
            }
        }

        Manager_Quest.Instance.CheckInventoryItemQuest();
        return false;
    }

    // =========================
    // CHECK ITEM (QUEST, CRAFT)
    // =========================
    public bool HasItem(string itemId, int amount)
    {
        int total = 0;

        foreach (var stack in items)
        {
            if (stack.item.id == itemId)
                total += stack.amount;
        }

        return total >= amount;
    }

    public ItemStack GetMainHandItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count)
            return null;

        return items[selectedIndex];
    }

    public ItemStack GetItemAtIndex(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;

        return items[index];
    }
    public int GetItemCount()
    {
        return items.Count;
    }

} 