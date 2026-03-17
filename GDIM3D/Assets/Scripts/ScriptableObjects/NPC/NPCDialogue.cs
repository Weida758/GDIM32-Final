using UnityEngine;

/// <summary>
/// Holds the data the DialogueSystem needs to start a conversation.
/// </summary>
public class NPCDialogue : MonoBehaviour
{
    public SO_NPC npcData;
    public DialogueNode startingNode;

    private QuestGiver questGiver;

    private void Awake()
    {
        questGiver = GetComponent<QuestGiver>();
    }
    
    /// <summary>
    /// Returns the correct dialogue node to use right now
    /// If a QuestGiver is attached, it uses the dialogue node of the QuestGiver
    /// </summary>
    /// <returns></returns>
    public DialogueNode GetCurrentStartingNode()
    {
        if (questGiver != null)
        {
            return questGiver.GetCurrentDialogueNode();
        }

        return startingNode;
    }
}