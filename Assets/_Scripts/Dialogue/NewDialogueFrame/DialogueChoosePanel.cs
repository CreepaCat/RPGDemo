using NewDialogueFrame;
using RPGDemo.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoosePanel : BasePanel
{
    // [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Transform optionsRoot;
    [SerializeField] Button optionPrefab;

    PlayerCanversant playerCanversant;

    protected override void Awake()
    {
        base.Awake();
        playerCanversant = PlayerCanversant.GetPlayerCanversant();
    }
    private void Start()
    {
        HideMe();
    }

    private void OnEnable()
    {
        playerCanversant.OnChooseDialogue += ShowMe;
    }
    private void OnDisable()
    {
        playerCanversant.OnChooseDialogue -= ShowMe;
    }

    private void ShowMe()
    {
        UIManager.Instance.OpenPanel<DialogueChoosePanel>();
        DrawUI();
    }
    private void HideMe()
    {
        //base.OnHide();
        UIManager.Instance.ClosePanel<DialogueChoosePanel>();
    }
    private void CloseMe()
    {
        CloseSelf();
    }





    public void DrawUI()
    {
        foreach (Transform child in optionsRoot)
        {
            Destroy(child.gameObject);
        }

        //根据当前对话对象可选择的对话分支显示面板
        var target = playerCanversant.GetCurrentCanversantTarget();

        foreach (var dialogue in target.GetComponent<CanversantTargetInteractable>().GetVildaDialogues())
        {
            var btn = Instantiate(optionPrefab, optionsRoot);
            TextMeshProUGUI tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
            tmp.text = dialogue.displayName;
            btn.onClick.AddListener(() =>
            {
                //HideMe();
                CloseMe();
                playerCanversant.StartDialogue(dialogue, target);
            });
        }
        var btnClose = Instantiate(optionPrefab, optionsRoot);
        TextMeshProUGUI text = btnClose.GetComponentInChildren<TextMeshProUGUI>();
        text.text = "离开";
        btnClose.onClick.AddListener(() =>
        {
            // HideMe();
            CloseMe();
            //将当前对话对象置空
            playerCanversant.SetCurrentCanversantTarget(null);
        });
    }
}
