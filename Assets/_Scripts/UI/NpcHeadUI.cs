using MyNodeEditor.Extension.Dialogue;
using RPGDemo.Quests;
using RPGDemo.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NpcHeadUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] Color inProgressColor;
    [SerializeField] private Sprite canAcceptIcon;
    [SerializeField] private Sprite canCompleteIcon;
    [SerializeField] private Sprite inProgressIcon;
    [SerializeField] private TextMeshProUGUI tmp_npcName;
    [SerializeField] DialogueRole dialogueRole;

    AIQuestHandler aIQuestHandler;
    PlayerQuestHandler playerQuestHandler;
    Color originIconColor;

    private void Awake()
    {
        aIQuestHandler = GetComponentInParent<AIQuestHandler>();
        playerQuestHandler = PlayerQuestHandler.GetInstance();
        iconImage.enabled = false;
        tmp_npcName.text = dialogueRole.speakerName;
        originIconColor = iconImage.color;
    }

    void OnEnable()
    {
        //目前某个任务的接取只看角色等级和前置任务是否完成
        //todo:若还有物品要求可在之后继续添加监听背包物品变动事件
        playerQuestHandler.OnQuestProgressChanged += UpdateIcon;
        playerQuestHandler.OnQuestCompleted += UpdateIcon;

        var baseStats = playerQuestHandler?.GetComponent<BaseStats>();
        if (baseStats != null)
        {
            baseStats.OnLevelUp += OnConditionChanged;
        }

    }
    void OnDisable()
    {
        if (playerQuestHandler == null) return;
        playerQuestHandler.OnQuestProgressChanged -= UpdateIcon;
        playerQuestHandler.OnQuestCompleted -= UpdateIcon;

        var baseStats = playerQuestHandler.GetComponent<BaseStats>();
        if (baseStats != null)
        {
            baseStats.OnLevelUp -= OnConditionChanged;
        }

    }
    void Start()
    {
        UpdateIcon(null);
    }

    private void OnConditionChanged()
    {
        UpdateIcon(null);
    }


    public void UpdateIcon(QuestSO questSO)
    {
        var questStatus = aIQuestHandler.GetAIQuestListStatus();
        if (questStatus == AIQuestListStatus.InProgress)
        {
            iconImage.color = inProgressColor;
        }
        else
        {
            iconImage.color = originIconColor;

        }
        switch (questStatus)
        {
            case AIQuestListStatus.CanGiveQuest:
                iconImage.enabled = true;
                iconImage.sprite = canAcceptIcon;
                break;
            case AIQuestListStatus.CanCompleteQuest:
                iconImage.enabled = true;
                iconImage.sprite = canCompleteIcon;
                break;
            case AIQuestListStatus.InProgress:
                iconImage.enabled = true;
                iconImage.sprite = inProgressIcon;
                break;

            default:
                iconImage.enabled = false;
                break;
        }
    }
}
