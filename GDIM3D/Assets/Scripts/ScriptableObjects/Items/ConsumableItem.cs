using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Scriptable Objects/ConsumableItem")]
public class ConsumableItem : ItemData
{
    public List<ItemEffectEntry> itemEffects;

    public void Use(Player player)
    {
        foreach (var effect in itemEffects)
        {
            effect.itemEffect.Apply(player, effect.value, effect.duration);
        }
    }
    
}

[System.Serializable]
public struct ItemEffectEntry
{
    public ItemEffect itemEffect;
    public float value;
    public float duration;
}
