using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGDemo.Quests
{
    public class QuestPanelUI:MonoBehaviour
    {
        [SerializeField] private QuestSlotUI questSlotPrefab;
        [SerializeField] private Transform questSlotsRoot;
        [SerializeField] TextMeshProUGUI txt_noQuest;
        
        PlayerQuestHandler _playerQuestHandler;
        QuestStatus _currentSelectedQuest = null;
        
        int _lastSelectedIndex = -1;

        public Action OnRefreshPanel;
        private void Awake()
        {
            _playerQuestHandler = PlayerQuestHandler.GetInstance();
        
          

        }

        private void OnEnable()
        {
            _playerQuestHandler.OnQuestProgressChanged += RefreshPanel;
        }

        private void OnDisable()
        {
            _playerQuestHandler.OnQuestProgressChanged -= RefreshPanel;
            
        
        }
        private void Start()
        {
            
            txt_noQuest.gameObject.SetActive(!_playerQuestHandler.HasAnyQuest());
            
            SetCurrentSelectedSlot(0);
        }

        private void RefreshPanel(QuestSO quest)
        {
            Debug.Log("QuestInfoPanelUI RefreshPanel");
            DrawQuestSlots();
            
            OnRefreshPanel?.Invoke();
        }



        public QuestStatus CurrentSelectedQuest=>_currentSelectedQuest;

        public void SetCurrentSelectedSlot(int newSelectedindex)
        {
            if(!_playerQuestHandler.HasAnyQuest()) return;
            if(newSelectedindex<0) return;
            if(_lastSelectedIndex == newSelectedindex) return;
            _lastSelectedIndex = newSelectedindex;
            _currentSelectedQuest = _playerQuestHandler.GetActiveQuests()[newSelectedindex];
            
            DrawQuestSlots();
           // GetComponentInChildren<QuestInfoPanelUI>().UpdateUI();
           OnRefreshPanel?.Invoke();
        }


        private void DrawQuestSlots()
        {
            foreach (Transform child in questSlotsRoot)
            {
                Destroy(child.gameObject);
            }

            if( !_playerQuestHandler.GetActiveQuests().Any())
            {
                //若为空，显示一个空任务UI
                 txt_noQuest.gameObject.SetActive(true);
                 _currentSelectedQuest = null;
                 _lastSelectedIndex = -1;
                 return;
            }
            
            int slotIndex = 0;
            foreach (var questStatus in _playerQuestHandler.GetActiveQuests())
            {
                QuestSlotUI questSlot = Instantiate(questSlotPrefab, questSlotsRoot);
               // bool isSelected = (ReferenceEquals(quest, _currentSelectedQuest));
                bool isSelected = _lastSelectedIndex == slotIndex;
                questSlot.Setup(questStatus,slotIndex,isSelected);
               
                //if(slotIndex == cu)
                slotIndex++;
           
            }
            //若之前没选择任务，现在有任务，默认选第一个
            if(_lastSelectedIndex == -1 )
            {
                SetCurrentSelectedSlot(0);
            }
            
            
        }
    }
}