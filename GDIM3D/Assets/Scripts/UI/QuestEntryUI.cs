using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// A single row in the quest list inside QuestMenuUI.
/// </summary>
public class QuestEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statusIndicator;
    [SerializeField] private Button button;
    [SerializeField] private Image backgroundImage;

    [Header("Colors")]
    [SerializeField] private Color activeColor = new Color(1f, 1f, 1f, 0.1f);
    [SerializeField] private Color readyColor = new Color(0.3f, 0.85f, 0.3f, 0.15f);
    [SerializeField] private Color completedColor = new Color(0.5f, 0.5f, 0.5f, 0.1f);

    private QuestInstance questInstance;
    private QuestMenuUI menuUI;

    public void Setup(QuestInstance qi, QuestMenuUI menu)
    {
        questInstance = qi;
        menuUI = menu;

        nameText.text = qi.questData.questName;

        switch (qi.status)
        {
            case QuestStatus.Active:
                statusIndicator.text = "...";
                statusIndicator.color = new Color(1f, 0.84f, 0f);
                if (backgroundImage) backgroundImage.color = activeColor;
                break;
            case QuestStatus.ReadyToTurnIn:
                statusIndicator.text = "!";
                statusIndicator.color = new Color(0.3f, 0.85f, 0.3f);
                if (backgroundImage) backgroundImage.color = readyColor;
                break;
            case QuestStatus.Completed:
                statusIndicator.text = "\u2713";
                statusIndicator.color = new Color(0.5f, 0.5f, 0.5f);
                if (backgroundImage) backgroundImage.color = completedColor;
                nameText.fontStyle = FontStyles.Strikethrough;
                break;
        }

        // Tracking indicator
        if (QuestManager.instance != null && QuestManager.instance.TrackedQuest == qi)
        {
            nameText.text = "\u25B6 " + nameText.text;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => menuUI.SelectQuest(questInstance));
    }
}