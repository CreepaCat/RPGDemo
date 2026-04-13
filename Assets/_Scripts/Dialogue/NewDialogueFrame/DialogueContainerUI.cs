using System;
using MyNodeEditor.Extension.Dialogue;
using RPGDemo.UI;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace NewDialogueFrame
{
    [RequireComponent(typeof(CanvasGroup))]
    public class DialogueContainerUI : BasePanel
    {
        [SerializeField] private TextMeshProUGUI speakerName;
        [SerializeField] private TextMeshProUGUI dialogueContent;
        [SerializeField] private Image avatar;

        [SerializeField] private Button dialogueOptionPrefab;
        [SerializeField] private Button nextButton;

        [SerializeField] private Transform dialogueContentRoot;
        [SerializeField] private Transform dialogueOptionsRoot;

        PlayerCanversant _playerCanversant;

        protected override void Awake()
        {
            base.Awake();
            _playerCanversant = Player.GetInstance().GetComponent<PlayerCanversant>();

        }

        private void OnEnable()
        {
            _playerCanversant.OnDialogueTreeUpdated += UpdateUI;
            _playerCanversant.OnDialogueBegun += ShowMe;
            _playerCanversant.OnDialogueEnded += HideMe;

            nextButton.onClick.AddListener(() => OnChoose(0));
        }

        private void OnDisable()
        {
            _playerCanversant.OnDialogueTreeUpdated -= UpdateUI;
            _playerCanversant.OnDialogueBegun -= ShowMe;
            _playerCanversant.OnDialogueEnded -= HideMe;
        }

        private void Start()
        {
            OnHide();
        }

        public void ShowMe()
        {

            UIManager.Instance.OpenPanel<DialogueContainerUI>();
        }

        public void HideMe()
        {
            UIManager.Instance.ClosePanel<DialogueContainerUI>();

        }

        private void UpdateUI(DialogueNodeData runningNodeData)
        {

            ShowDialogueOptionPanel(false);
            speakerName.text = runningNodeData.speakerName;
            dialogueContent.text = runningNodeData.dialogueContent;
            avatar.sprite = runningNodeData.avatar;

            if (runningNodeData?.dialogueRole?.speakerName == "玩家")
            {
                //说明后面的节点不是选项对话
                return;
            }


            var validOptions = runningNodeData.GetValidateOptions();
            //合格选项数大于1，说明要显示所有选项，否则显示next按钮
            if (validOptions.Count > 0)
            {
                ShowDialogueOptionPanel(true);

                foreach (var option in validOptions)
                {
                    Button btnOption = Instantiate(dialogueOptionPrefab, dialogueOptionsRoot);
                    btnOption.GetComponentInChildren<TextMeshProUGUI>().text = option.OptionText;
                    btnOption.onClick.AddListener(() => OnChoose(option.OptionIndex));
                }
            }
        }


        private void OnChoose(int currentIndex)
        {
            Debug.Log("选择了选项" + currentIndex);
            _playerCanversant.OnChoose(currentIndex);
        }


        /// <summary>
        /// 显示对话选项
        /// </summary>
        /// <param name="shouldShow"></param>
        private void ShowDialogueOptionPanel(bool shouldShow)
        {
            //每次显示选项前，先清除之前创建的子选项

            foreach (Transform item in dialogueOptionsRoot)
            {
                Destroy(item.gameObject);
            }

            dialogueOptionsRoot.gameObject.SetActive(shouldShow);
            nextButton.gameObject.SetActive(!shouldShow);
        }
    }
}
