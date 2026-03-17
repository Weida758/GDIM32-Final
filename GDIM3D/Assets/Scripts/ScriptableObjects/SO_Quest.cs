using UnityEngine;


[CreateAssetMenu(fileName = "Quest", menuName = "Scriptable Objects/Quest")]
public class SO_Quest : ScriptableObject
{
    [Header("Quest Info")]
    public int questId;
    public string questName;
    [TextArea(3, 6)] public string questDescription;

    [Header("Objectives")] 
    public QuestObjective[] objectives;
    
    [Header("Rewards")] 
    public int goldReward;

}
