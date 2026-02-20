using UnityEngine;

public abstract class ItemEffect : ScriptableObject
{
    public float value;
    public float duration;
    public abstract void Apply(Player player);
}
