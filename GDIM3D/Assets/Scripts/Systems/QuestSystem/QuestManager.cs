using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Evaluates inventory-based objectives everytime the inventory changes
/// </summary>
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance
    {
        get;
        private set;
    }

    private readonly List<QuestInstance> allQuests = new List<QuestInstance>();
    
    public IReadOnlyList<QuestInstance> AllQuests => allQuests;
    
    /// <summary>
    /// Quest that the player is currently tracking, null if none
    /// </summary>
    public QuestInstance TrackedQuest { get; private set; }
    
    // ---------- Events -----------
    /// <summary>
    /// The event that occurs when a quest is accepted, updated, completed, or tracking changes
    /// </summary>
    public event Action OnQuestStateChanged;

    private PlayerInventory inventory;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        if (Locator.instance != null && Locator.instance.player != null)
        {
            inventory = Locator.instance.player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.OnInventoryChanged += EvaluateAllActiveQuests;
            }
        }
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= EvaluateAllActiveQuests;
        }
    }

    /// <summary>
    /// Has the player already accepted or completed this quest
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public bool HasQuest(SO_Quest quest)
    {
        return GetInstance(quest) != null;
    }

    public QuestInstance GetInstance(SO_Quest quest)
    {
        for (int i = 0; i < allQuests.Count; i++)
        {
            if (allQuests[i].questData == quest) return allQuests[i];
        }

        return null;
    }

    /// <summary>
    /// Accept a new quest and returns the new QuestInstance, or null if already accepted
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public QuestInstance AcceptQuest(SO_Quest quest)
    {
        if (HasQuest(quest))
        {
            Debug.LogWarning($"Quest '{quest.questName}'  is already accepted.");
            return null;
        }

        var instance = new QuestInstance(quest);
        allQuests.Add(instance);

        if (TrackedQuest == null)
        {
            TrackedQuest = instance;
        }

        Debug.Log($"QuestManager Accepted quest: {quest.questName}");

        EvaluateQuest(instance);

        OnQuestStateChanged?.Invoke();
        return instance;
    }
    
    /// <summary>
    /// Try to complete a quest and remove required items from inventory if required to deliver
    /// to the NPC. Grant rewards to the player and return true on success.
    /// </summary>
    /// <param name="quest"></param>
    /// <returns></returns>
    public bool CompleteQuest(SO_Quest quest)
    {
        var instance = GetInstance(quest);
        if (instance == null || instance.status == QuestStatus.Completed)
        {
            return false;
        }

        EvaluateQuest(instance);

        if (!instance.AreAllObjectivesMet())
        {
            Debug.Log($"QuestManager Quest '{quest.questName}' objectives not met.");
            return false;
        }
        
        // Remove delivered item from inventory
        foreach (var obj in quest.objectives)
        {
            if (obj.type == ObjectiveType.Deliver || obj.type == ObjectiveType.Collect)
            {
                RemoveItemsFromInventory(obj.requiredItem, obj.requiredAmount);
                
            }
        }

        instance.status = QuestStatus.Completed;
        
        //Grant rewards
        PlayerWallet.Instance.AddGold(quest.goldReward);
        Debug.Log($"QuestManager Completed quest: {quest.questName}, +{quest.goldReward} gold");
        
        // set next quest as tracked 
        if (TrackedQuest == instance)
        {
            TrackedQuest = FindNextActiveQuest();
        }

        OnQuestStateChanged?.Invoke();
        return true;

    }

    public void SetTrackedQuest(QuestInstance quest)
    {
        TrackedQuest = quest;
        OnQuestStateChanged?.Invoke();
    }

    public void RecordKill(string enemyID)
    {
        // Placeholder
        // match enemyID against objective.requiredItem.itemName or a dedicated field.
        bool changed = false;
        foreach (var qi in allQuests)
        {
            if (qi.status != QuestStatus.Active) continue;
            for (int i = 0; i < qi.questData.objectives.Length; i++)
            {
                var obj = qi.questData.objectives[i];
                if (obj.type == ObjectiveType.Kill && obj.requiredItem != null
                                                   && obj.requiredItem.itemName == enemyID)
                {
                    changed |= qi.SetProgress(i, qi.progress[i] + 1);
                }
            }
        }
 
        if (changed)
        {
            UpdateStatuses();
            OnQuestStateChanged?.Invoke();
        }
    }
    
    // ------------ Helpers ---------------
    public List<QuestInstance> GetActiveQuests()
    {
        return allQuests.FindAll(q =>
            q.status == QuestStatus.Active || q.status == QuestStatus.ReadyToTurnIn);
        
    }

    public List<QuestInstance> GetCompletedQuests()
    {
        return allQuests.FindAll(q => q.status == QuestStatus.Completed);
    }
    
    // --------------------------------------

    private void EvaluateAllActiveQuests()
    {
        bool anyChanged = false;
        foreach (var qi in allQuests)
        {
            if (qi.status == QuestStatus.Active || qi.status == QuestStatus.ReadyToTurnIn)
            {
                anyChanged |= EvaluateQuest(qi);
            }
        }

        if (anyChanged)
        {
            OnQuestStateChanged?.Invoke();
        }
    }


    private bool EvaluateQuest(QuestInstance qi)
    {
        if (inventory == null) return false;
        bool changed = false;

        for (int i = 0; i < qi.questData.objectives.Length; i++)
        {
            var obj = qi.questData.objectives[i];

            if (obj.type == ObjectiveType.Collect || obj.type == ObjectiveType.Deliver)
            {
                int count = CountItemInInventory(obj.requiredItem);
                changed |= qi.SetProgress(i, count);
            
            }
            
        }

        if (qi.status != QuestStatus.Completed)
        {
            QuestStatus newStatus = qi.AreAllObjectivesMet()
                ? QuestStatus.ReadyToTurnIn
                : QuestStatus.Active;

            if (qi.status != newStatus)
            {
                qi.status = newStatus;
                changed = true;
            }
        }

        return changed;
    }

    private void UpdateStatuses()
    {
        foreach (var qi in allQuests)
        {
            if (qi.status == QuestStatus.Completed) continue;
            qi.status = qi.AreAllObjectivesMet() ? QuestStatus.ReadyToTurnIn : QuestStatus.Active;
        }
        
    }

    private int CountItemInInventory(ItemData item)
    {
        int total = 0;
        foreach (var slot in inventory.slots)
        {
            if (!slot.isSlotEmpty && slot.item == item)
            {
                total += slot.quantity;
            }
        }

        return total;
    }

    private void RemoveItemsFromInventory(ItemData item, int amount)
    {
        int remaining = amount;
        for (int i = 0; i < inventory.slots.Count && remaining > 0; i++)
        {
            var slot = inventory.slots[i];
            if (slot.isSlotEmpty || slot.item != item) continue;

            int toRemove = Mathf.Min(remaining, slot.quantity);
            inventory.RemoveItem(i, toRemove);
            remaining -=  toRemove;
        }
    }


    private QuestInstance FindNextActiveQuest()
    {
        foreach (var qi in allQuests)
        {
            if (qi.status == QuestStatus.Active || qi.status == QuestStatus.ReadyToTurnIn)
            {
                return qi;
            }
        }

        return null;
        
    }
    
    

    
    
}
