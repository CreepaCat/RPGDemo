using System;
using RPGDemo.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GamePanelsUI : BasePanel
{
    // [SerializeField] ToggleGroup toggleGroup;
    [SerializeField] private Toggle toggle_questPanel;
    [SerializeField] private Toggle toggle_inventoryPanel;
    [SerializeField] private Toggle toggle_guidePanel;
    [SerializeField] CanvasGroup questPanelCg;
    [SerializeField] CanvasGroup inventoryPanelCg;
    [SerializeField] CanvasGroup guidePanelCg;
    [SerializeField] Key GAME_PANEL_KEY = Key.I;

    protected override void Awake()
    {
        base.Awake();

    }

    private void Start()
    {

        HideMe();
        toggle_inventoryPanel.isOn = true;
    }

    private void OnEnable()
    {
        toggle_questPanel.onValueChanged.AddListener((value) => SetCanvasGroup(questPanelCg, value));
        toggle_inventoryPanel.onValueChanged.AddListener((value) => SetCanvasGroup(inventoryPanelCg, value));
        toggle_guidePanel.onValueChanged.AddListener((value) => SetCanvasGroup(guidePanelCg, value));
    }

    private void OnDisable()
    {
        toggle_questPanel.onValueChanged.RemoveAllListeners();
        toggle_inventoryPanel.onValueChanged.RemoveAllListeners();
        toggle_guidePanel.onValueChanged.RemoveAllListeners();
    }


    private void Update()
    {
        if (Keyboard.current[GAME_PANEL_KEY].wasPressedThisFrame)
        {
            if (GetComponent<CanvasGroup>().alpha > 0)
            {
                CloseMe();

            }
            else
            {
                ShowMe();

            }
        }


    }

    private void SetCanvasGroup(CanvasGroup canvasGroup, bool isShow)
    {
        canvasGroup.alpha = isShow ? 1 : 0;
        canvasGroup.interactable = isShow;
        canvasGroup.blocksRaycasts = isShow;
    }

    //PUBLIC

    public void ShowMe()
    {
        UIManager.Instance.OpenPanel<GamePanelsUI>();
    }

    public void HideMe()
    {
        UIManager.Instance.ClosePanel<GamePanelsUI>();
    }

    public void CloseMe()
    {
        CloseSelf();
    }

}
