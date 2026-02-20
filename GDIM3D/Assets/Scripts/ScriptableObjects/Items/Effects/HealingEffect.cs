using UnityEngine;

[CreateAssetMenu(fileName = "HealingEffect", menuName = "Scriptable Objects/Effects/HealingEffect")]
public class HealingEffect : ItemEffect
{
    public override void Apply(Player player)
    {
        player.healthComponent.Heal(value);
    }
}
