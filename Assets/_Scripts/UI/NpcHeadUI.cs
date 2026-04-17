using DG.Tweening;
using MyNodeEditor.Extension.Dialogue;
using RPGDemo.Quests;
using RPGDemo.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//todo:如果一个任务涉及多个NPC的多段对话，则在任务中的每个NPC都会显示相同的任务状态
//todo:应将任务步骤拆分，只按当前步骤显示头顶对话状态
//todo:一段任务分为多个步骤，每个步骤都有发布者和奖励者，一个步骤为一个任务环节闭环
/// <summary>
/// NPC头顶任务状态显示
/// </summary>
public class NpcHeadUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private SpriteRenderer miniMapIcon;
    [SerializeField] Color inProgressColor;
    [SerializeField] private Sprite canAcceptIcon;
    [SerializeField] private Sprite canCompleteIcon;
    [SerializeField] private Sprite inProgressIcon;
    [SerializeField] private TextMeshProUGUI tmp_npcName;
    [SerializeField] DialogueRole dialogueRole;

    [Header("tween动画参数")]
    [SerializeField] Transform doYoyoTarget;
    public float amplitude = 2f;        // 上下移动的幅度（距离）
                                        //public float amplitude = 2f;        // 上下移动的幅度（距离）
    public float duration = 1.2f;       // 单程时间（越小越快）
    public Ease easeType = Ease.InOutSine;   // 缓动类型（推荐 InOutSine 最自然）
    private Vector3 startPos;


    AIQuestHandler aIQuestHandler;
    PlayerQuestHandler playerQuestHandler;
    Color originIconColor;


    private void Awake()
    {
        aIQuestHandler = GetComponentInParent<AIQuestHandler>();
        playerQuestHandler = PlayerQuestHandler.GetInstance();
        iconImage.enabled = false;
        miniMapIcon.enabled = false;

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

        startPos = doYoyoTarget.position;
        PlayYoYo();
    }

    private void OnDestroy()
    {
        doYoyoTarget.DOKill();
    }

    public void PlayYoYo()
    {
        // 计算目标位置（在初始位置的正上方或下方）
        Vector3 targetPos = startPos + new Vector3(0, amplitude, 0);

        // 使用 DOMoveY 实现上下移动 + 无限循环
        doYoyoTarget.DOMoveY(targetPos.y, duration)
            .SetEase(easeType)                    // 平滑缓动
            .SetLoops(-1, LoopType.Yoyo);         // -1 = 无限循环，Yoyo = 往返

    }

    private void OnConditionChanged()
    {
        UpdateIcon(null);
    }


    public void UpdateIcon(QuestSO questSO)
    {
        if (miniMapIcon == null || iconImage == null) return;
        var questStatus = aIQuestHandler.GetAIQuestListStatus();
        if (questStatus == AIQuestListStatus.InProgress)
        {
            iconImage.color = inProgressColor;
            miniMapIcon.color = inProgressColor;
        }
        else
        {
            iconImage.color = originIconColor;
            miniMapIcon.color = originIconColor;

        }
        Debug.Log(transform.name + ":" + questStatus.ToString());
        switch (questStatus)
        {

            case AIQuestListStatus.CanGiveQuest:
                iconImage.enabled = true;
                iconImage.sprite = canAcceptIcon;
                miniMapIcon.enabled = true;
                miniMapIcon.sprite = canAcceptIcon;
                break;
            case AIQuestListStatus.CanCompleteQuest:
                iconImage.enabled = true;
                iconImage.sprite = canCompleteIcon;
                miniMapIcon.enabled = true;
                miniMapIcon.sprite = canCompleteIcon;
                break;
            case AIQuestListStatus.InProgress:
                iconImage.enabled = true;
                iconImage.sprite = inProgressIcon;
                miniMapIcon.enabled = true;
                miniMapIcon.sprite = inProgressIcon;

                break;

            default:
                iconImage.enabled = false;
                miniMapIcon.enabled = false;
                break;
        }
    }
}
