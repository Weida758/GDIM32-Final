using UnityEngine;

/// <summary>
/// Allows for different branch of dialogues based on quest state.
/// Handles quest accepting and turning in.
/// </summary>
public class QuestGiver : MonoBehaviour
{
    public SO_Quest quest;

    [Header("Dialogue Nodes Per State")]
    [Tooltip("Dialogue shown when offering the quest for the first time.")]
    public DialogueNode offerNode;

    [Tooltip("Dialogue shown when the player has the required item and can turn in")]
    public DialogueNode inProgressNode;

    [Tooltip("Dialogue shown when the player has the required item and can turn in")]
    public DialogueNode turnInNode;

    [Tooltip("Dialogue shown after quest is completed")]
    public DialogueNode completedNode;

    private NPCDialogue npcDialogue;


    private void Awake()
    {
        npcDialogue = GetComponent<NPCDialogue>();
    }

    public DialogueNode GetCurrentDialogueNode()
    {
        Debug.Log($"[QuestGiver] quest is null? {quest == null}");
    
        if (quest == null) return npcDialogue != null ? npcDialogue.startingNode : null;

        var qm = QuestManager.instance;
        Debug.Log($"[QuestGiver] QuestManager.Instance is null? {qm == null}");
    
        if (qm == null) return offerNode;

        QuestInstance qi = qm.GetInstance(quest);
        Debug.Log($"[QuestGiver] QuestInstance is null? {qi == null}");

        if (qi == null)
            return offerNode;

        Debug.Log($"[QuestGiver] qi.status = {qi.status}");

        switch (qi.status)
        {
            case QuestStatus.Active:
                return inProgressNode != null ? inProgressNode : offerNode;
            case QuestStatus.ReadyToTurnIn:
                return turnInNode != null ? turnInNode : offerNode;
            case QuestStatus.Completed:
                return completedNode != null ? completedNode : offerNode;
            default:
                return offerNode;
        }
    }
}
