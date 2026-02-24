using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler,
    IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI quantityText;


    [DisplayOnly] public int slotIndex;
    [DisplayOnly] public InventoryUI inventoryUI;

    private InventorySlot slotData;
    private bool isHovering;

    private void Update()
    {
        if (!isHovering) return;
        if (Input.GetKeyDown(KeyCode.T))
        {
            inventoryUI.inventory.ConsumeItem(slotIndex);
        }
    }
    
    public void SetData(InventorySlot slotData, int index)
    {
        this.slotData = slotData;
        this.slotIndex = index;

        if (this.slotData == null || this.slotData.isSlotEmpty)
        {
            iconImage.enabled = false;
            quantityText.text = "";
        }
        else
        {
            iconImage.enabled = true;
            iconImage.sprite = this.slotData.item.itemSprite;
            quantityText.text = this.slotData.quantity >= 1 ? this.slotData.quantity.ToString() : "";
        }
    }
    // ---------- Left click Drags --------------
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (inventoryUI.isPicking) return;
        if (slotData == null || slotData.isSlotEmpty) return;
        inventoryUI.StartDrag(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (inventoryUI.isPicking) return;
        if (inventoryUI.dragIcon != null)
        {
            inventoryUI.dragIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (inventoryUI.isPicking) return;
        inventoryUI.EndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (inventoryUI.isPicking) return;
        var from = inventoryUI.draggedSlot;
        if (from != null && from != this)
        {
            inventoryUI.inventory.SwapSlots(from.slotIndex, this.slotIndex);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (inventoryUI.isPicking)
            {
                inventoryUI.PlacePickedItems(this);
                return;
            }
            
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            inventoryUI.BeginOrIncrementPick(this);
        }
    }
}
