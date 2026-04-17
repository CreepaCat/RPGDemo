using RPGDemo.UI;
using UnityEngine;
using UnityEngine.UI;
using RPGDemo.SceneManagement;
using System.Collections;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : BasePanel
{
    [SerializeField] Button btn_continue;
    [SerializeField] Button btn_newGame;
    [SerializeField] Button btn_settings;
    [SerializeField] Button btn_credits;
    [SerializeField] Button btn_quitGame;

    private void Start()
    {
        SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();

        btn_newGame.onClick.AddListener(
            () =>
            {
                savingWrapper.DeleteFile();
                savingWrapper.LoadNewGame();
            }
        );
        btn_settings.onClick.AddListener(() =>
        {
            //打开setting面板
            UIManager.Instance.OpenPanel<SettingsPanel>();
        });

        btn_credits.onClick.AddListener(() =>
        {
            //打开credits面板
            UIManager.Instance.OpenPanel<CreditsPanel>();
        });

        btn_quitGame.onClick.AddListener(QuitGame);


        ShowMe();
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;

#endif
        Application.Quit();
    }
    public void CloseMe()
    {
        CloseSelf();
    }

    public void ShowMe()
    {
        UIManager.Instance.OpenPanel<MainMenu>();

    }

    public void HideMe()
    {
        UIManager.Instance.ClosePanel<MainMenu>();
    }


    public override void OnShow()
    {
        base.OnShow();
        SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
        if (btn_continue.gameObject.activeSelf)
        {
            btn_continue.onClick.RemoveAllListeners();
            btn_continue.onClick.AddListener(() =>
            {
                savingWrapper.LoadScene();
            });

        }
        btn_continue.gameObject.SetActive(savingWrapper.HasFile());
    }




}
