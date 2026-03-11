using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private DialogueUI dialogueUI;

    private DialogueNode currentNode;
    private SO_NPC currentNPC;
    private int lineIndex;

    public bool IsDialogueActive { get; private set; }

    private OrbitCamera orbitCamera;

    private void Start()
    {
        orbitCamera = FindFirstObjectByType<OrbitCamera>();
        dialogueUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// Called by Player when the crosshair is on an NPC and the player clicks.
    /// </summary>
    public void StartDialogue(SO_NPC npc, DialogueNode startNode)
    {
        if (IsDialogueActive) return;
        if (startNode == null) return;

        currentNPC = npc;
        currentNode = startNode;
        lineIndex = 0;
        IsDialogueActive = true;

        SetPlayerControlsEnabled(false);

        dialogueUI.gameObject.SetActive(true);
        dialogueUI.Initialize(this);
        ShowNextLine();
    }

    /// <summary>
    /// Called by DialogueUI when the player clicks/presses to advance.
    /// Shows the next NPC line, or shows player options, or ends dialogue.
    /// </summary>
    public void AdvanceDialogue()
    {
        if (!IsDialogueActive) return;

        if (lineIndex < currentNode._lines.Length)
        {
            ShowNextLine();
        }
        else
        {
            if (currentNode._playerReplyOptions != null && currentNode._playerReplyOptions.Length > 0)
            {
                dialogueUI.ShowPlayerOptions(currentNode._playerReplyOptions);
            }
            else
            {
                EndDialogue();
            }
        }
    }

    private void ShowNextLine()
    {
        // Use the node's npc if assigned, otherwise fall back to the NPC that started the conversation
        string speakerName = currentNode.npc != null ? currentNode.npc.NPC_name : currentNPC.NPC_name;
        string line = currentNode._lines[lineIndex];
        dialogueUI.ShowNPCDialogue(speakerName, line);
        lineIndex++;
    }

    /// <summary>
    /// Called by DialogueUI when the player picks one of the reply buttons.
    /// Transitions to the corresponding next DialogueNode.
    /// </summary>
    public void SelectPlayerOption(int optionIndex)
    {
        if (optionIndex < 0 || currentNode._npcReplies == null 
                             || optionIndex >= currentNode._npcReplies.Length)
        {
            EndDialogue();
            return;
        }

        DialogueNode nextNode = currentNode._npcReplies[optionIndex];

        if (nextNode == null)
        {
            EndDialogue();
            return;
        }

        currentNode = nextNode;
        lineIndex = 0;

        // New node has NPC lines
        if (currentNode._lines != null && currentNode._lines.Length > 0)
        {
            dialogueUI.HidePlayerOptions();
            ShowNextLine();
        }
        // New node has no NPC lines but has player options
        else if (currentNode._playerReplyOptions != null && currentNode._playerReplyOptions.Length > 0)
        {
            dialogueUI.ShowPlayerOptions(currentNode._playerReplyOptions);
        }
        else
        {
            EndDialogue();
        }
    }

    public void EndDialogue()
    {
        IsDialogueActive = false;
        currentNode = null;
        currentNPC = null;
        lineIndex = 0;

        dialogueUI.ResetUI();
        dialogueUI.gameObject.SetActive(false);

        SetPlayerControlsEnabled(true);
    }

    private void SetPlayerControlsEnabled(bool enabled)
    {
        if (orbitCamera != null)
            orbitCamera.enabled = enabled;

        Cursor.visible = !enabled;
        Cursor.lockState = enabled ? CursorLockMode.Locked : CursorLockMode.None;
    }
}