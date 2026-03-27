using System.Collections;
using System.Collections.Generic;
using TMPro;
using RPGDemo.Inventories;
using UnityEngine;

public class SideMessageBox : MonoBehaviour
{
    public enum MessageType
    {
        Normal = 0,
        Pickup = 1,
        Drop = 2,
        QuestCompleted = 3,
        QuestAccepted = 4,
        Reward = 5
    }

    [Header("UI")]
    [SerializeField] private RectTransform messageRoot;
    [SerializeField] private TextMeshProUGUI messageTemplate;

    [Header("Display")]
    [SerializeField] private int maxVisibleCount = 6;
    [SerializeField] private float messageLifeTime = 2.2f;
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private float scrollDuration = 0.2f;
    [SerializeField] private float messageSpacing = 6f;

    [Header("Message Style")]
    [SerializeField] private string pickupPrefix = "拾取";
    [SerializeField] private string dropPrefix = "丢弃";
    [SerializeField] private string questAcceptedPrefix = "接受任务";
    [SerializeField] private string questCompletedPrefix = "任务完成";
    [SerializeField] private string rewardPrefix = "获得奖励";

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pickupColor = new Color(0.65f, 1f, 0.65f, 1f);
    [SerializeField] private Color dropColor = new Color(1f, 0.72f, 0.72f, 1f);
    [SerializeField] private Color questColor = new Color(1f, 0.94f, 0.56f, 1f);
    [SerializeField] private Color rewardColor = new Color(0.62f, 0.92f, 1f, 1f);

    private static SideMessageBox _instance;
    private readonly List<MessageEntry> _activeEntries = new();

    private class MessageEntry
    {
        public TextMeshProUGUI text;
        public RectTransform rect;
        public CanvasGroup canvasGroup;
        public Coroutine lifeRoutine;
        public Coroutine moveRoutine;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        if (messageRoot == null)
        {
            messageRoot = transform as RectTransform;
        }

        if (messageTemplate != null)
        {
            messageTemplate.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        foreach (Transform child in messageRoot)
        {
            Destroy(child.gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }

    public static void Show(MessageType type, string content)
    {
        if (_instance == null || string.IsNullOrWhiteSpace(content)) return;
        _instance.PushTypedMessage(type, content);
    }

    public static void ShowPickup(InventoryItem item, int amount)
    {
        if (item == null || amount <= 0) return;
        Show(MessageType.Pickup, $"{item.GetDisplayName()} x{amount}");
    }

    public static void ShowDrop(InventoryItem item, int amount)
    {
        if (item == null || amount <= 0) return;
        Show(MessageType.Drop, $"{item.GetDisplayName()} x{amount}");
    }

    public static void ShowQuestCompleted(string questName)
    {
        if (string.IsNullOrWhiteSpace(questName)) return;
        Show(MessageType.QuestCompleted, questName);
    }
    public static void ShowQuestAccepted(string questName)
    {
        if (string.IsNullOrWhiteSpace(questName)) return;
        Show(MessageType.QuestAccepted, questName);
    }


    public static void ShowReward(string rewardName, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(rewardName) || amount <= 0) return;

        string payload = amount > 1 ? $"{rewardName} x{amount}" : rewardName;
        Show(MessageType.Reward, payload);
    }

    public void PushMessage(string message)
    {
        PushMessage(message, normalColor);
    }

    public void PushMessage(string message, Color color)
    {
        if (string.IsNullOrWhiteSpace(message) || messageTemplate == null || messageRoot == null)
        {
            return;
        }

        TextMeshProUGUI textObj = Instantiate(messageTemplate, messageRoot);
        textObj.gameObject.SetActive(true);
        textObj.text = message;
        textObj.color = color;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        float preferredHeight = Mathf.Max(messageTemplate.preferredHeight, textObj.preferredHeight);
        textRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
        textRect.anchoredPosition = Vector2.zero;

        CanvasGroup canvasGroup = textObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = textObj.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        MessageEntry entry = new MessageEntry
        {
            text = textObj,
            rect = textRect,
            canvasGroup = canvasGroup
        };

        _activeEntries.Add(entry);

        RepositionEntries(animate: true);

        if (_activeEntries.Count > maxVisibleCount)
        {
            RemoveEntry(_activeEntries[0]);
        }

        entry.lifeRoutine = StartCoroutine(FadeAndRecycle(entry));
    }

    private void PushTypedMessage(MessageType type, string content)
    {
        ResolveStyle(type, out string prefix, out Color prefixColor);

        if (string.IsNullOrWhiteSpace(prefix))
        {
            PushMessage(content, normalColor);
            return;
        }

        string prefixHex = ColorUtility.ToHtmlStringRGBA(prefixColor);
        string text = $"<color=#{prefixHex}>{prefix}</color> {content}";
        PushMessage(text, normalColor);
    }

    private void ResolveStyle(MessageType type, out string prefix, out Color prefixColor)
    {
        prefix = string.Empty;
        prefixColor = normalColor;

        switch (type)
        {
            case MessageType.Pickup:
                prefix = pickupPrefix;
                prefixColor = pickupColor;
                break;
            case MessageType.Drop:
                prefix = dropPrefix;
                prefixColor = dropColor;
                break;
            case MessageType.QuestAccepted:
                prefix = questAcceptedPrefix;
                prefixColor = questColor;
                break;
            case MessageType.QuestCompleted:
                prefix = questCompletedPrefix;
                prefixColor = questColor;
                break;
            case MessageType.Reward:
                prefix = rewardPrefix;
                prefixColor = rewardColor;
                break;
        }
    }

    private IEnumerator FadeAndRecycle(MessageEntry entry)
    {
        yield return new WaitForSeconds(messageLifeTime);

        if (entry == null || entry.text == null || entry.canvasGroup == null)
        {
            yield break;
        }

        if (fadeDuration > 0f)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float ratio = Mathf.Clamp01(t / fadeDuration);
                entry.canvasGroup.alpha = 1f - ratio;
                yield return null;
            }
        }

        RemoveEntry(entry);
    }

    private void RemoveEntry(MessageEntry entry)
    {
        if (entry == null)
        {
            return;
        }

        if (entry.lifeRoutine != null)
        {
            StopCoroutine(entry.lifeRoutine);
            entry.lifeRoutine = null;
        }

        if (entry.moveRoutine != null)
        {
            StopCoroutine(entry.moveRoutine);
            entry.moveRoutine = null;
        }

        _activeEntries.Remove(entry);

        if (entry.text != null)
        {
            Destroy(entry.text.gameObject);
        }

        RepositionEntries(animate: true);
    }

    private void RepositionEntries(bool animate)
    {
        float y = 0f;

        for (int i = _activeEntries.Count - 1; i >= 0; i--)
        {
            MessageEntry entry = _activeEntries[i];
            if (entry == null || entry.rect == null) continue;

            Vector2 target = new Vector2(entry.rect.anchoredPosition.x, y);

            if (animate)
            {
                if (entry.moveRoutine != null)
                {
                    StopCoroutine(entry.moveRoutine);
                }

                entry.moveRoutine = StartCoroutine(ScrollToPosition(entry, target));
            }
            else
            {
                entry.rect.anchoredPosition = target;
            }

            y += entry.rect.rect.height + messageSpacing;
        }
    }

    private IEnumerator ScrollToPosition(MessageEntry entry, Vector2 target)
    {
        if (entry == null || entry.rect == null)
        {
            yield break;
        }

        Vector2 start = entry.rect.anchoredPosition;

        if (scrollDuration <= 0f)
        {
            entry.rect.anchoredPosition = target;
            entry.moveRoutine = null;
            yield break;
        }

        float t = 0f;
        while (t < scrollDuration)
        {
            t += Time.deltaTime;
            float ratio = Mathf.Clamp01(t / scrollDuration);
            ratio = Mathf.SmoothStep(0f, 1f, ratio);
            entry.rect.anchoredPosition = Vector2.LerpUnclamped(start, target, ratio);
            yield return null;
        }

        entry.rect.anchoredPosition = target;
        entry.moveRoutine = null;
    }

    [ContextMenu("Test Pickup Message")]
    private void TestPickupMessage()
    {
        Show(MessageType.Pickup, "测试道具 x1");
    }

    [ContextMenu("Test Drop Message")]
    private void TestDropMessage()
    {
        Show(MessageType.Drop, "测试道具 x1");
    }

    [ContextMenu("Test QuestCompleted Message")]
    private void TestQuestCompletedMessage()
    {
        ShowQuestCompleted("第一章:新的旅程");
    }

    [ContextMenu("Test Reward Message")]
    private void TestRewardMessage()
    {
        ShowReward("金币", 300);
    }
}
