using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUI : MonoBehaviour
{
    [Header("NPC Dialogue")]
    [SerializeField] private GameObject npcDialoguePanel; // The Image holding the dialogue box
    [SerializeField] private TextMeshProUGUI nameText;// Displays speaker name
    [SerializeField] private TextMeshProUGUI npcDialogueText; // Displays current dialogue line

    [Header("Player Options")]
    [SerializeField] private GameObject playerOptionsPanel;  // The Image holding the option buttons
    [SerializeField] private Button[] optionButtons; // The 3 buttons (assign in inspector)

    private DialogueSystem dialogueSystem;
    private bool waitingForAdvance;

    /// <summary>
    /// Called by DialogueSystem when a new conversation starts.
    /// </summary>
    public void Initialize(DialogueSystem system)
    {
        dialogueSystem = system;
        npcDialoguePanel.SetActive(false);
        HidePlayerOptions();
    }

    private void Update()
    {
        if (waitingForAdvance)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
            {
                waitingForAdvance = false;
                dialogueSystem.AdvanceDialogue();
            }
        }
    }

    /// <summary>
    /// Show a single NPC dialogue line with the speaker's name.
    /// </summary>
    public void ShowNPCDialogue(string speakerName, string dialogue)
    {
        playerOptionsPanel.SetActive(false);
        npcDialoguePanel.SetActive(true);

        nameText.text = speakerName;
        npcDialogueText.text = dialogue;

        waitingForAdvance = true;
    }

    /// <summary>
    /// Show the player's reply options as clickable buttons. Hides the NPC dialogue panel.
    /// </summary>
    public void ShowPlayerOptions(string[] options)
    {
        waitingForAdvance = false;
        npcDialoguePanel.SetActive(false);
        playerOptionsPanel.SetActive(true);

        nameText.text = "You";

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < options.Length)
            {
                optionButtons[i].gameObject.SetActive(true);

                TextMeshProUGUI btnText = optionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null)
                    btnText.text = options[i];
                
                optionButtons[i].onClick.RemoveAllListeners();
                int capturedIndex = i;
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(capturedIndex));
            }
            else
            {
                // Hide unused buttons
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Hide all player option buttons and clear their listeners.
    /// </summary>
    public void HidePlayerOptions()
    {
        playerOptionsPanel.SetActive(false);

        for (int i = 0; i < optionButtons.Length; i++)
        {
            optionButtons[i].onClick.RemoveAllListeners();
            optionButtons[i].gameObject.SetActive(false);
        }
    }

    private void OnOptionSelected(int index)
    {
        HidePlayerOptions();
        dialogueSystem.SelectPlayerOption(index);
    }

    /// <summary>
    /// Reset all UI elements when dialogue ends.
    /// </summary>
    public void ResetUI()
    {
        waitingForAdvance = false;
        npcDialoguePanel.SetActive(false);
        HidePlayerOptions();
        nameText.text = "";
        npcDialogueText.text = "";
    }
}