using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Shows a brief notification banner when a quest is accepted or completed.
/// Fades in, holds, then fades out.
/// </summary>
public class QuestNotificationUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI notificationText;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float holdDuration = 2f;
    [SerializeField] private float fadeOutDuration = 0.5f;

    private Coroutine activeCoroutine;
    private QuestManager qm;

    private int lastActiveCount;
    private int lastCompletedCount;

    private void Start()
    {
        canvasGroup.alpha = 0f;

        qm = QuestManager.instance;
        if (qm != null)
        {
            lastActiveCount = qm.GetActiveQuests().Count;
            lastCompletedCount = qm.GetCompletedQuests().Count;
            qm.OnQuestStateChanged += CheckForNotification;
        }
    }

    private void OnDestroy()
    {
        if (qm != null)
            qm.OnQuestStateChanged -= CheckForNotification;
    }

    private void CheckForNotification()
    {
        if (qm == null) return;

        int currentActive = qm.GetActiveQuests().Count;
        int currentCompleted = qm.GetCompletedQuests().Count;

        if (currentCompleted > lastCompletedCount)
        {
            ShowNotification("Quest Completed!");
        }
        else if (currentActive > lastActiveCount)
        {
            ShowNotification("New Quest Accepted!");
        }

        lastActiveCount = currentActive;
        lastCompletedCount = currentCompleted;
    }

    public void ShowNotification(string message)
    {
        if (activeCoroutine != null)
            StopCoroutine(activeCoroutine);

        notificationText.text = message;
        activeCoroutine = StartCoroutine(NotificationRoutine());
    }

    private IEnumerator NotificationRoutine()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        yield return new WaitForSecondsRealtime(holdDuration);

        t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        canvasGroup.alpha = 0f;
        activeCoroutine = null;
    }
}