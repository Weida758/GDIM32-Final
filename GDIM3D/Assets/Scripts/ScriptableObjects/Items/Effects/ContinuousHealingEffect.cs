using UnityEngine;

[CreateAssetMenu(fileName = "ContinuousHealingEffect", menuName = "Scriptable Objects/Effects/ContinuousHealingEffect")]
public class ContinuousHealingEffect : ItemEffect
{
    public float timePerTick;
    public override void Apply(Player player, float value, float duration)
    {
        player.healthComponent.ContinuousHeal(value, timePerTick, duration);
    }
}