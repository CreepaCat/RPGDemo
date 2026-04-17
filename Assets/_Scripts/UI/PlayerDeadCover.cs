using DG.Tweening;
using RPGDemo.SceneManagement;
using RPGDemo.UI;
using UnityEngine;

public class PlayerDeadCover : BasePanel
{
    [Header("tweening 参数")]
    [SerializeField] Transform tweenTarget;
    [SerializeField] Vector3 endScale = Vector3.one;
    [SerializeField] float durantion = 1f;
    Player player;

    protected override void Awake()
    {
        base.Awake();
        player = Player.GetInstance();
    }

    private void Start()
    {
        HideMe();
        player.Health.OnDeath += ShowMe;
    }

    private void OnDestroy()
    {
        player.Health.OnDeath -= ShowMe;
    }

    [ContextMenu("show me")]
    public void ShowMeFromInspector()
    {
        ShowMe();
    }




    private void ShowMe()
    {
        UIManager.Instance.OpenPanel<PlayerDeadCover>();
        tweenTarget.localScale = Vector3.one;
        tweenTarget.DOScale(endScale, durantion).SetEase(Ease.Linear).SetLoops(1).OnComplete(() =>
        {
            //加载出生点
            SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
            CloseMe();
            savingWrapper.Respawn();

        });
    }

    private void HideMe()
    {
        UIManager.Instance.ClosePanel<PlayerDeadCover>();
    }

    private void CloseMe()
    {
        CloseSelf();
    }
}
