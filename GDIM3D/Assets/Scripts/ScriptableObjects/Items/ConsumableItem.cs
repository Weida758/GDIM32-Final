using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConsumableItem", menuName = "Scriptable Objects/ConsumableItem")]
public class ConsumableItem : ItemData
{
    public List<ItemEffect> itemEffects;

    public void Use(Player player)
    {
        foreach (var effect in itemEffects)
        {
            effect.Apply(player);
        }
    }
    
}
