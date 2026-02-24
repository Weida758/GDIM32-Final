using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour, IPointerClickHandler
{

    public PlayerInventory inventory;
    [FormerlySerializedAs("slotUIs")] public InventorySlotUI[] slotUis;

    public Image dragIcon;
    public TextMeshProUGUI dragQuantityText;

    // --------------- Picking -----------------
    [HideInInspector] public InventorySlotUI draggedSlot;
    [HideInInspector] public bool isPicking;
    [HideInInspector] public int pickSourceIndex = -1;
    [HideInInspector] public ItemData pickedItem;
    [HideInInspector] public int pickedQuantity;

    [Header("Pick Action Settings")] 
    [SerializeField] private float pickInitialDelay = 0.4f;
    [SerializeField] private float pickRepeatRate = 0.1f;
    private float pickHoldTimer;
    private float pickRepeatTimer;
    private bool isHoldingRightClick;
    
    private int contextSlotIndex;

    private static InventoryUI instance;
    
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    private void Start()
    {
        inventory = FindFirstObjectByType<PlayerInventory>();

        for (int i = 0; i < slotUis.Length; i++)
        {
            slotUis[i].inventoryUI = this;
            slotUis[i].slotIndex = i;
        }

        Refresh();
    }

    private void OnEnable()
    {
        
        inventory.OnInventoryChanged += Refresh;
        
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        Debug.Log("Actual Slots: " + inventory.slots.Count + " UI slots: " + slotUis.Length);
        for (int i = 0; i < slotUis.Length; i++)
        {
            slotUis[i].SetData(inventory.slots[i], i);
        }
        
    }

    private void Update()
    {
        if (isPicking && dragIcon)
        {
            dragIcon.transform.position = Input.mousePosition;
        }
        
        // Increment while holding right click
        if (isPicking && isHoldingRightClick)
        {
            pickHoldTimer += Time.unscaledDeltaTime;

            if (pickHoldTimer >= pickInitialDelay)
            {
                pickRepeatTimer += Time.unscaledDeltaTime;
                if (pickRepeatTimer >= pickRepeatRate)
                {
                    pickRepeatTimer = 0f;
                    TryIncrementPick();
                }
            }
        }

        if (isHoldingRightClick && Input.GetMouseButtonUp(1))
        {
            isHoldingRightClick = false;
        }
        
    }
    
    /// <summary>
    /// Begin or increment the slot that is picked
    /// </summary>
    /// <param name="slot"> The individual inventory slot UI that is picked</param>
    public void BeginOrIncrementPick(InventorySlotUI slot)
    {
        InventorySlot slotData = inventory.slots[slot.slotIndex];

        if (!isPicking)
        {
            // Start new pick from this slot
            if (slotData.isSlotEmpty) return;

            isPicking = true;
            pickSourceIndex = slot.slotIndex;
            pickedItem = slotData.item;
            pickedQuantity = 0;
            
            TryIncrementPick();
            // Show drag icon
            dragIcon.sprite = pickedItem.itemSprite;
            dragIcon.enabled = true;
            if (dragQuantityText)
            {
                dragQuantityText.enabled = true;
            }
            UpdateDragQuantityText();
            
            isHoldingRightClick = true;
            pickHoldTimer = 0f;
            pickRepeatTimer = 0f;
        }
        else if (slot.slotIndex == pickSourceIndex)
        {
            TryIncrementPick();

            isHoldingRightClick = true;
            pickHoldTimer = 0f;
            pickRepeatTimer = 0f;
        }
    }

    private void TryIncrementPick()
    {
        InventorySlot sourceSlot = inventory.slots[pickSourceIndex];

        if (sourceSlot.isSlotEmpty || sourceSlot.quantity <= 0) return;

        pickedQuantity++;
        sourceSlot.quantity--;

        if (sourceSlot.quantity <= 0)
        {
            sourceSlot.item = null;
            sourceSlot.quantity = 0;
            
        }

        inventory.NotifyChanged();
        UpdateDragQuantityText();

    }
    /// <summary>
    /// Called when left clicking a slot while picking and place the item
    /// </summary>
    /// <param name="targetSlot"></param>
    public void PlacePickedItems(InventorySlotUI targetSlot)
    {
        if (!isPicking) return;

        int targetIndex = targetSlot.slotIndex;
        var target = inventory.slots[targetIndex];

        if (targetIndex == pickSourceIndex)
        {
            ReturnPickedItems();
            return;
        }

        if (target.isSlotEmpty)
        {
            target.item = pickedItem;
            target.quantity = pickedQuantity;
            EndPick();
        }
        else if (target.item == pickedItem && target.item.isStackable)
        {
            int canAdd = Mathf.Min(pickedQuantity, target.item.maxStackSize - target.quantity);
            target.quantity += canAdd;
            pickedQuantity -= canAdd;

            if (pickedQuantity <= 0)
            {
                EndPick();
            }
            else
            {
                UpdateDragQuantityText();
            }
        }
        else
        {
            //Handling case for differnt items: swap the items in hand and the items picked
            ItemData swapItem = target.item;
            int swapQuantity = target.quantity;

            target.item = pickedItem;
            target.quantity = pickedQuantity;

            pickedItem = swapItem;
            pickedQuantity = swapQuantity;
            pickSourceIndex = targetIndex;

            dragIcon.sprite = pickedItem.itemSprite;
            UpdateDragQuantityText();

        }

        inventory.NotifyChanged();
    }

    public void ReturnPickedItems()
    {
        if (!isPicking) return;
        var sourceSlot = inventory.slots[pickSourceIndex];

        if (sourceSlot.isSlotEmpty)
        {
            sourceSlot.item = pickedItem;
            sourceSlot.quantity = pickedQuantity;
        }
        else if (sourceSlot.item == pickedItem)
        {
            sourceSlot.quantity += pickedQuantity;
        }
        else
        {
            var emptySlot = inventory.slots.Find(slot => slot.isSlotEmpty);
            if (emptySlot != null)
            {
                emptySlot.item = pickedItem;
                emptySlot.quantity = pickedQuantity;
            }
            else
            {
                Debug.LogWarning("No empty slot to return picked item");
            }
        }

        EndPick();
       inventory.NotifyChanged();
    }

    private void EndPick()
    {
        isPicking = false;
        pickSourceIndex = -1;
        pickedItem = null;
        pickedQuantity = 0;
        isHoldingRightClick = false;

        dragIcon.enabled = false;
        if (dragQuantityText)
        {
            dragQuantityText.text = "";
            dragQuantityText.enabled = false;
        }
    }

    private void UpdateDragQuantityText()
    {
        if (dragQuantityText)
        {
            dragQuantityText.text = pickedQuantity > 1 ? pickedQuantity.ToString() : "";
        }
    }

    public void StartDrag(InventorySlotUI slot)
    {
        draggedSlot = slot;
        dragIcon.sprite = inventory.slots[slot.slotIndex].item.itemSprite;
        dragIcon.enabled = true;
    }

    public void EndDrag()
    {
        dragIcon.enabled = false;
        draggedSlot = null;
    }
    
    public void OnConsumeItem()
    {
        inventory.ConsumeItem(contextSlotIndex);
    }

    public void OnDrop()
    {
        inventory.DropItem(contextSlotIndex);
        
    }

    public void OnDropAll()
    {
        inventory.DropItem(contextSlotIndex, inventory.slots[contextSlotIndex].quantity);
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
