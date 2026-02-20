using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> slots = new List<InventorySlot>();
    [SerializeField] private int inventorySize = 16;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        
    }


}
