using UnityEngine;

public enum ObjectiveType
{
    Collect, Deliver, Kill
}

[System.Serializable]
public class QuestObjective
{
    public ObjectiveType type;

    [Tooltip("The item the player needs to collect or deliver.")]
    public ItemData requiredItem;

    public int requiredAmount;
    public string description;
}
