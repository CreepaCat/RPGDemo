using RPGDemo.Quests;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class QuestTrackUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_questName;
    [SerializeField] TextMeshProUGUI tmp_questLevel;
    [SerializeField] TextMeshProUGUI tmp_questStatus;
    [SerializeField] QuestObjectiveSlotUI questObjectiveSlotPrefab;
    [SerializeField] Transform slotsRoot;

    CanvasGroup canvasGroup;


    PlayerQuestHandler playerQuestHandler;
    private QuestStatus trackingQuest => playerQuestHandler.GetTrackingQuest();

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        playerQuestHandler = PlayerQuestHandler.GetInstance();
    }
    private void OnEnable()
    {
        playerQuestHandler.OnQuestProgressChanged += DrawUI;
        playerQuestHandler.OnQuestCompleted += DrawUI;
    }

    private void OnDisable()
    {
        playerQuestHandler.OnQuestProgressChanged -= DrawUI;
        playerQuestHandler.OnQuestCompleted -= DrawUI;
    }

    private void Start()
    {
        // trackingQuest.quest.questName = "测试任务";
        DrawUI(trackingQuest?.quest);
    }

    private void Update()
    {
        if (trackingQuest != null && !trackingQuest.IsFinished() && canvasGroup.alpha < 1f)
        {
            ShowMe();
        }
        else if ((trackingQuest == null || trackingQuest.IsFinished()) && canvasGroup.alpha > 0f)
        {
            HideMe();
        }

    }



    public void ShowMe()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        DrawUI(trackingQuest?.quest);
    }

    public void HideMe()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void DrawUI(QuestSO questSO)
    {
        if (trackingQuest == null || trackingQuest.IsFinished())
        {
            HideMe();
            return;
        }
        if (trackingQuest.quest != questSO) return;

        foreach (Transform child in slotsRoot)
        {
            Destroy(child.gameObject);
        }
        //  ShowMe();

        tmp_questName.text = trackingQuest.quest.GetQuestName();
        tmp_questLevel.text = "Lv. " + trackingQuest.quest.GetQuestLevel().ToString();

        switch (trackingQuest.progress)
        {
            case QuestProgress.Active:
                tmp_questStatus.text = "进行中";
                break;
            case QuestProgress.Completed:
                tmp_questStatus.text = "可提交";
                break;
            case QuestProgress.Finished:
                tmp_questStatus.text = "已完结";
                break;
            default:
                tmp_questStatus.text = "";
                break;
        }

        foreach (var objective in trackingQuest.quest.GetQuestObjectives())
        {
            var objectiveSlot = Instantiate(questObjectiveSlotPrefab, slotsRoot);
            objectiveSlot.Setup(trackingQuest.GetObjectiveStatus(objective));
        }

    }

}
