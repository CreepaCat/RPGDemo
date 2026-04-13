using UnityEngine;
using RPGDemo.UI;
using UnityEngine.UI;
using RPGDemo.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections;
public class PauseMenu : BasePanel
{

    [SerializeField] Button btn_backToGame;
    [SerializeField] Button btn_saveGame;
    [SerializeField] Button btn_quitToMainMenu;

    private void Start()
    {

        btn_backToGame.onClick.AddListener(HideMe);
        btn_saveGame.onClick.AddListener(
            () =>
            {
                SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
                savingWrapper.Save();
                HideMe();
            }
        );
        btn_quitToMainMenu.onClick.AddListener(() =>
        {
            //先自动保存
            SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
            savingWrapper.Save();
            //加载主菜单场景
            Fader fader = FindFirstObjectByType<Fader>();
            savingWrapper.Save();
            StartCoroutine(LoadMainMenu());
            fader.StartFadeOutIn();
        });


        HideMe();
    }

    IEnumerator LoadMainMenu()
    {
        yield return SceneManager.LoadSceneAsync(0);
    }




    public override void OnShow()
    {
        base.OnShow();
        //暂停时间
        Time.timeScale = 0f;

    }

    public override void OnHide()
    {
        base.OnHide();
        //恢复时间
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
    //PRIVATE

    private void ShowMe()
    {
        UIManager.Instance.OpenPanel<PauseMenu>();
    }

    private void HideMe()
    {
        UIManager.Instance.ClosePanel<PauseMenu>();
    }

    private void CloseMe()
    {
        CloseSelf();
    }
}
