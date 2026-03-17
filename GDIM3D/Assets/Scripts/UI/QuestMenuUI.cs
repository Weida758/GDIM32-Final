using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestMenuUI : MonoBehaviour
{
    [Header("Quest List (Left Panel)")]
    [SerializeField] private Transform questListContent;
    [SerializeField] private QuestEntryUI questEntryPrefab;

    [Header("Quest Detail (Right Panel)")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI objectivesText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private UnityEngine.UI.Button trackButton;
    [SerializeField] private TextMeshProUGUI trackButtonText;

    [Header("Section Headers")]
    [SerializeField] private TextMeshProUGUI activeHeaderText;
    [SerializeField] private TextMeshProUGUI completedHeaderText;

    private QuestManager qm;
    private readonly List<QuestEntryUI> spawnedEntries = new List<QuestEntryUI>();
    private QuestInstance selectedQuest;

    private void OnEnable()
    {
        qm = QuestManager.instance;
        if (qm != null)
            qm.OnQuestStateChanged += RebuildList;

        RebuildList();
    }

    private void OnDisable()
    {
        if (qm != null)
            qm.OnQuestStateChanged -= RebuildList;
    }

    private void RebuildList()
    {
        foreach (var entry in spawnedEntries)
        {
            if (entry != null)
                Destroy(entry.gameObject);
        }
        spawnedEntries.Clear();

        if (qm == null) return;

        var active = qm.GetActiveQuests();
        var completed = qm.GetCompletedQuests();

        // Active section
        if (activeHeaderText != null)
        {
            activeHeaderText.gameObject.SetActive(active.Count > 0);
            activeHeaderText.text = $"Active ({active.Count})";
        }

        foreach (var qi in active)
        {
            var entry = Instantiate(questEntryPrefab, questListContent);
            entry.Setup(qi, this);
            spawnedEntries.Add(entry);
        }

        // Completed section
        if (completedHeaderText != null)
        {
            completedHeaderText.gameObject.SetActive(completed.Count > 0);
            completedHeaderText.text = $"Completed ({completed.Count})";
        }

        foreach (var qi in completed)
        {
            var entry = Instantiate(questEntryPrefab, questListContent);
            entry.Setup(qi, this);
            spawnedEntries.Add(entry);
        }

        if (selectedQuest == null && detailPanel != null)
            detailPanel.SetActive(false);
        else
            ShowDetail(selectedQuest);
    }

    /// <summary>Called by QuestEntryUI when clicked.</summary>
    public void SelectQuest(QuestInstance qi)
    {
        selectedQuest = qi;
        ShowDetail(qi);
    }

    private void ShowDetail(QuestInstance qi)
    {
        if (qi == null || detailPanel == null) return;

        detailPanel.SetActive(true);

        titleText.text = qi.questData.questName;
        descriptionText.text = qi.questData.questDescription;

        // Objectives
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int i = 0; i < qi.questData.objectives.Length; i++)
        {
            var obj = qi.questData.objectives[i];
            int current = qi.progress[i];
            int required = obj.requiredAmount;
            bool done = current >= required;

            string check = done ? "<color=#4CAF50>\u2713</color>" : "\u2022";
            sb.AppendLine($"  {check}  {obj.description}  ({current}/{required})");
        }
        objectivesText.text = sb.ToString();

        // Rewards
        rewardText.text = $"Gold: {qi.questData.goldReward}";

        // Status
        switch (qi.status)
        {
            case QuestStatus.Active:
                statusText.text = "In Progress";
                statusText.color = new Color(1f, 0.84f, 0f);
                break;
            case QuestStatus.ReadyToTurnIn:
                statusText.text = "Ready to Turn In";
                statusText.color = new Color(0.3f, 0.85f, 0.3f);
                break;
            case QuestStatus.Completed:
                statusText.text = "Completed";
                statusText.color = new Color(0.5f, 0.5f, 0.5f);
                break;
            default:
                statusText.text = "";
                break;
        }

        // Track button
        if (trackButton != null)
        {
            bool canTrack = qi.status != QuestStatus.Completed;
            trackButton.gameObject.SetActive(canTrack);

            bool isTracked = qm.TrackedQuest == qi;
            trackButtonText.text = isTracked ? "Tracking" : "Track Quest";
            trackButton.interactable = !isTracked;

            trackButton.onClick.RemoveAllListeners();
            trackButton.onClick.AddListener(() =>
            {
                qm.SetTrackedQuest(qi);
                ShowDetail(qi);
            });
        }
    }
}