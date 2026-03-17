using UnityEngine;
using TMPro;

/// <summary>
/// Displays the currently tracked quest
/// </summary>
public class QuestTrackerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private TextMeshProUGUI objectivesText;

    private QuestManager qm;

    private void Start()
    {
        qm = QuestManager.instance;
        if (qm != null)
            qm.OnQuestStateChanged += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (qm != null)
            qm.OnQuestStateChanged -= Refresh;
    }

    private void Refresh()
    {
        if (qm == null || qm.TrackedQuest == null || qm.TrackedQuest.status == QuestStatus.Completed)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        QuestInstance qi = qm.TrackedQuest;
        questNameText.text = qi.questData.questName;

        // Build objectives list
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < qi.questData.objectives.Length; i++)
        {
            var obj = qi.questData.objectives[i];
            int current = qi.progress[i];
            int required = obj.requiredAmount;
            bool done = current >= required;

            if (done)
                sb.Append("<s>");

            sb.Append($"  {obj.description}  ({current}/{required})");

            if (done)
                sb.Append("</s>");

            if (i < qi.questData.objectives.Length - 1)
                sb.Append("\n");
        }

        objectivesText.text = sb.ToString();
    }
}