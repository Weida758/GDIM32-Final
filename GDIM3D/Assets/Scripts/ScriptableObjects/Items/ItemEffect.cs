using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public abstract void Apply(Player player, float value, float duration);
}
