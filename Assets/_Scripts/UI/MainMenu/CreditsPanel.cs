using RPGDemo.UI;
using UnityEngine;
using UnityEngine.UI;

public class CreditsPanel : BasePanel
{
    [SerializeField] Button btn_close;

    private void Start()
    {
        btn_close.onClick.AddListener(CloseMe);

        HideMe();
    }

    public void CloseMe()
    {
        CloseSelf();
    }

    public void ShowMe()
    {
        UIManager.Instance.OpenPanel<CreditsPanel>();

    }

    public void HideMe()
    {
        UIManager.Instance.ClosePanel<CreditsPanel>();
    }



}
