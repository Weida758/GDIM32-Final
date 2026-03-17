using System;

public enum QuestStatus
{
    NotStarted,
    Active,
    ReadyToTurnIn,
    Completed
}

/// <summary>
/// The representation of a quest the player has accepted
/// and its progress & status
/// </summary>

[Serializable]
public class QuestInstance
{
    public SO_Quest questData;
    public QuestStatus status;

    /// <summary>
    /// Current progress count per questData.objective
    /// </summary>
    public int[] progress;

    public QuestInstance(SO_Quest quest)
    {
        questData = quest;
        status = QuestStatus.Active;
        progress = new int[quest.objectives.Length];
    }
    
    /// <summary>
    /// Update progress for a specific objective index
    /// Returns true if value is changed
    /// </summary>
    /// <param name="objectiveIndex"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool SetProgress(int objectiveIndex, int value)
    {
        if (objectiveIndex < 0 || objectiveIndex >= progress.Length) return false;

        int clamped = Math.Min(value, questData.objectives[objectiveIndex].requiredAmount);
        if (progress[objectiveIndex] == clamped) return false;


        progress[objectiveIndex] = clamped;
        return true;
    }

    public bool AreAllObjectivesMet()
    {
        for (int i = 0; i < questData.objectives.Length; i++)
        {
            if (progress[i] < questData.objectives[i].requiredAmount)
                return false;
        }

        return true;
    }
}
