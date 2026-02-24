using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [DisplayOnly] public ItemData item;
    [DisplayOnly] public int quantity;

    public InventorySlot(ItemData item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public bool isSlotEmpty => item == null;
    

}
