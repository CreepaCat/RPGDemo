using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomMessageBox : MonoBehaviour
{
    public enum BottomMessageType
    {
        Custom = 0,
        InventorySpaceNotEnough = 1,
        ManaNotEnough = 2,
        GoldNotEnough = 3
    }

    [Header("UI")]
    [SerializeField] private RectTransform messageRoot;
    [SerializeField] private Image messageTemplate;
    //  [SerializeField] private TextMeshProUGUI messageTemplate;

    [Header("Display")]
    [SerializeField] private int maxVisibleCount = 3;
    [SerializeField] private float messageLifeTime = 1.8f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float scrollDuration = 0.18f;
    [SerializeField] private float messageSpacing = 4f;

    [Header("Fixed Style")]
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color backgroundColor = Color.black;

    [Header("Default Messages")]
    [SerializeField] private string inventorySpaceNotEnoughText = "背包空间不足";
    [SerializeField] private string manaNotEnoughText = "法力值不足";
    [SerializeField] private string goldNotEnoughText = "金币不足";

    private static BottomMessageBox _instance;
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
            ApplyFixedStyle(messageTemplate);
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

    public static void Show(BottomMessageType type)
    {
        if (_instance == null) return;

        string text = _instance.ResolveTypeText(type);
        if (string.IsNullOrWhiteSpace(text)) return;

        _instance.PushMessage(text);
    }

    public static void Show(BottomMessageType type, string customContent)
    {
        if (_instance == null) return;

        if (type == BottomMessageType.Custom)
        {
            _instance.PushMessage(customContent);
            return;
        }

        if (string.IsNullOrWhiteSpace(customContent))
        {
            _instance.PushMessage(_instance.ResolveTypeText(type));
            return;
        }

        _instance.PushMessage(customContent);
    }

    public static void ShowCustom(string content)
    {
        if (_instance == null) return;
        _instance.PushMessage(content);
    }

    public static void ShowInventorySpaceNotEnough()
    {
        Show(BottomMessageType.InventorySpaceNotEnough);
    }

    public static void ShowManaNotEnough()
    {
        Show(BottomMessageType.ManaNotEnough);
    }

    public static void ShowGoldNotEnough()
    {
        Show(BottomMessageType.GoldNotEnough);
    }

    public void PushMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message) || messageTemplate == null || messageRoot == null)
        {
            return;
        }
        Image imgObj = Instantiate(messageTemplate, messageRoot);
        imgObj.gameObject.SetActive(true);

        TextMeshProUGUI textObj = imgObj.GetComponentInChildren<TextMeshProUGUI>();
        //textObj.gameObject.SetActive(true);
        textObj.text = message;
        ApplyFixedStyle(imgObj);

        textObj.ForceMeshUpdate();
        Vector2 size = textObj.GetPreferredValues(message); //计算自适应长框


        RectTransform imgRect = imgObj.GetComponent<RectTransform>();
        float preferredHeight = Mathf.Max(messageTemplate.preferredHeight, textObj.preferredHeight);
        imgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight + 10f);
        imgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x + 10f);


        imgRect.anchoredPosition = Vector2.zero;

        CanvasGroup canvasGroup = imgObj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = imgObj.gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        MessageEntry entry = new MessageEntry
        {
            text = textObj,
            rect = imgRect,
            canvasGroup = canvasGroup
        };

        _activeEntries.Add(entry);
        RepositionEntries(true);

        if (_activeEntries.Count > maxVisibleCount)
        {
            RemoveEntry(_activeEntries[0]);
        }

        entry.lifeRoutine = StartCoroutine(FadeAndRecycle(entry));
    }

    private string ResolveTypeText(BottomMessageType type)
    {
        switch (type)
        {
            case BottomMessageType.InventorySpaceNotEnough:
                return inventorySpaceNotEnoughText;
            case BottomMessageType.ManaNotEnough:
                return manaNotEnoughText;
            case BottomMessageType.GoldNotEnough:
                return goldNotEnoughText;
            default:
                return string.Empty;
        }
    }

    private void ApplyFixedStyle(Image imgObj)
    {

        if (imgObj == null) return;

        var textObj = imgObj.GetComponentInChildren<TextMeshProUGUI>();
        if (textObj == null) return;

        textObj.color = textColor;
        imgObj.color = backgroundColor;
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


        if (entry.rect != null)
        {
            Destroy(entry.rect.gameObject);
        }

        RepositionEntries(true);
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
            if (entry == null || entry.rect == null)
            {
                yield break;
            }
            entry.rect.anchoredPosition = Vector2.LerpUnclamped(start, target, ratio);
            yield return null;
        }

        entry.rect.anchoredPosition = target;
        entry.moveRoutine = null;
        // entry.canvasGroup.alpha = 0f;
    }

    [ContextMenu("Test Inventory Space Not Enough")]
    private void TestInventorySpaceNotEnough()
    {
        ShowInventorySpaceNotEnough();
    }

    [ContextMenu("Test Mana Not Enough")]
    private void TestManaNotEnough()
    {
        ShowManaNotEnough();
    }

    [ContextMenu("Test Gold Not Enough")]
    private void TestGoldNotEnough()
    {
        ShowGoldNotEnough();
    }
}
