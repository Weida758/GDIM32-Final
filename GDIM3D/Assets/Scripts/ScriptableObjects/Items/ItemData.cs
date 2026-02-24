using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    public int itemID;
    public Sprite itemSprite;
    public string itemName;
    public string itemDescription;
    public int maxStackSize;
    public bool isStackable;
}
