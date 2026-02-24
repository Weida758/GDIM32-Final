using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [DisplayOnly] public List<InventorySlot> slots;
    [SerializeField] private int inventorySize = 22;
    private Player player;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot(null, 0));
            Debug.Log("Added slots: " + i);
        }

        Debug.Log("Inventory Size: " + slots.Count);
        player = GetComponent<Player>();
        if (player)
        {
            Debug.Log("Player detected");
        }


    }

    public bool TryAddItem(ItemData item, int amount = 1)
    {

        if (item.isStackable)
        {
            foreach (var slot in slots)
            {
                if (slot.item == item && slot.quantity < item.maxStackSize)
                {
                    // for the current item slot, stack the collected item onto the slot
                    // if the canAdd is less than the amount being added, move onto
                    // next slot and continue stacking, else end function.
                    int canAdd = Mathf.Min(amount, item.maxStackSize - slot.quantity);
                    slot.quantity += canAdd;
                    amount -= canAdd;
                    if (amount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }

                }
            }
        }

        while (amount > 0)
        {
            // Check for the first slot that is empty
            var emptySlot = slots.FirstOrDefault(slot => slot.isSlotEmpty);
            if (emptySlot == null) return false;

            int toAdd = item.isStackable ? Mathf.Min(amount, item.maxStackSize) : 1;
            emptySlot.item = item;
            emptySlot.quantity = toAdd;
            amount -= toAdd;
        }

        NotifyChanged();
        return true;
    }

    public void RemoveItem(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return;
        var slot = slots[slotIndex];
        if (slot.isSlotEmpty) return;

        slot.quantity -= amount;
        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
            
        }

        NotifyChanged();

    }

    public void ConsumeItem(int slotIndex)
    {
        var slot = slots[slotIndex];
        if (slot.isSlotEmpty || slot.item is not ConsumableItem)
        {
            return;
        }
        
        ConsumableItem consumableItem = (ConsumableItem)slot.item;
        consumableItem.Use(player);
        RemoveItem(slotIndex, 1);
    }

    public void NotifyChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    public void SwapSlots(int from, int to)
    {
        if (from < 0 || to < 0 || from >= slots.Count || to >= slots.Count) return;

        var fromSlot = slots[from];
        var toSlot = slots[to];

        if (!fromSlot.isSlotEmpty && !toSlot.isSlotEmpty
                                  && fromSlot.item == toSlot.item && fromSlot.item.isStackable)
        {
            int canMove = Mathf.Min(fromSlot.quantity, toSlot.item.maxStackSize - toSlot.quantity);
            toSlot.quantity += canMove;
            fromSlot.quantity -= canMove;
            if (fromSlot.quantity <= 0)
            {
                fromSlot.item = null;
                fromSlot.quantity = 0;
            }
            
        }
        else
        {
            (slots[from], slots[to]) = (slots[to], slots[from]);
        }
        NotifyChanged();
    }

    public void DropItem(int slotIndex, int amount = 1)
    {
        var slot = slots[slotIndex];
        if (slot.isSlotEmpty) return;

        RemoveItem(slotIndex, amount);
    }


}
