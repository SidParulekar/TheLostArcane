using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance; //Created Inventory Manager singleton
    public int maxSlotSpace = 5;

    public List<InventorySlot> inventorySlots; //Stores the slots in the inventory

    public GameObject inventoryItemPrefab; //The inventory item that gets displayed in the inventory

    int selectedSlotIndex = -1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Looks for slot selected and if its not a slot thats currently selected, then replaces selected slot with new one
    /// </summary>
    /// <param name="selectedSlot"></param>
    public void ChangeSelectedSlot(InventorySlot selectedSlot)
    {       
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i] == selectedSlot)
            {
                if (selectedSlotIndex >= 0 && selectedSlotIndex!=i)
                {
                    inventorySlots[selectedSlotIndex].Unselect();
                }
                selectedSlotIndex = i;
                break;
            }
        }
    }

    /// <summary>
    /// Sees if item exists in inventory and if there is space in slot for stacking, if not then looks for free slot and adds item there
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++) // For stacking
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxSlotSpace && itemInSlot.item.stackable == true)
            {
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }

        }

        for (int i = 0; i < inventorySlots.Count; i++) // For finding free slots
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return true;
            }

        }

        return false;

    }

    /// <summary>
    /// Creates new instance of the inventory item prefab that is going to be added to inventory
    /// </summary>
    /// <param name="item"></param>
    /// <param name="slot"></param>
    private void SpawnNewItem(Item item, InventorySlot slot)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();

        inventoryItem.InitializeItem(item);
    }

    /// <summary>
    /// Calls function to use item and either decrements the stack count or destroys the item if count is 0
    /// </summary>
    public void UseItem()
    {
        InventorySlot selectedSlot = inventorySlots[selectedSlotIndex];

        InventoryItem itemInSlot = selectedSlot.GetComponentInChildren<InventoryItem>();       

        if (itemInSlot != null && PlayerController.Instance.ItemUseImpact(itemInSlot.item.collectible, itemInSlot.item.stat))
        {            
            itemInSlot.count--;
            itemInSlot.RefreshCount();

            if (itemInSlot.count <= 0)
            {
                Destroy(itemInSlot.gameObject);
            }
        }
    }
}
