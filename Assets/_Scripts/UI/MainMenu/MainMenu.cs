using RPGDemo.UI;
using UnityEngine;
using UnityEngine.UI;
using RPGDemo.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenu : BasePanel
{
    [SerializeField] Button btn_continue;
    [SerializeField] Button btn_newGame;
    [SerializeField] Button btn_settings;
    [SerializeField] Button btn_quitGame;

    private void Start()
    {
        SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
        Fader fader = FindFirstObjectByType<Fader>();
        // btn_continue.gameObject.SetActive(savingWrapper.HasFile());


        btn_newGame.onClick.AddListener(
            () =>
            {
                savingWrapper.DeleteFile();
                StartCoroutine(LoadNewGame());
                fader.StartFadeOutIn();
                //SceneManager.LoadSceneAsync(1);
            }
        );
        btn_settings.onClick.AddListener(() =>
        {
            //打开setting面板
            UIManager.Instance.OpenPanel<SettingsPanel>();
        });

        btn_quitGame.onClick.AddListener(() =>
        {
            Application.Quit();
        });



        ShowMe();
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

    IEnumerator LoadNewGame()
    {
        yield return SceneManager.LoadSceneAsync(1);
    }

    public override void OnShow()
    {
        base.OnShow();
        SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
        Fader fader = FindFirstObjectByType<Fader>();
        btn_continue.gameObject.SetActive(savingWrapper.HasFile());
        if (btn_continue.gameObject.activeSelf)
        {
            btn_continue.onClick.RemoveAllListeners();
            btn_continue.onClick.AddListener(() =>
            {
                savingWrapper.LoadScene();
                fader.StartFadeOutIn();
            });

        }
    }




}
