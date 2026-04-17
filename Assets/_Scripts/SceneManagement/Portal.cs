using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGDemo.SceneManagement
{
    /// <summary>
    /// 传送门对应的字母索引
    /// </summary>
    public enum PortalDestination
    {
        A, B, C, D, E, F
    }
    public class Portal : MonoBehaviour
    {
        [SerializeField] int _loadSceneIndex = -1; //目标场景索引
        [field: SerializeField] public Transform spawnPoint { get; private set; } //角色出生点
        [field: SerializeField] public int PortalIndex { get; private set; } = -1; //此传送门索引
        [field: SerializeField] public int TargetPortalIndex { get; private set; } = -1;//目标传送门索引
        [field: SerializeField] public PortalDestination Destination { get; private set; }
        [SerializeField] float _fadeOutTime = 1f, _fadeInTime = 0.5f, _waitTime = 1f;
        [SerializeField] bool isInSceneTransition = false;

        [Header("tween动画参数")]
        [SerializeField] Transform doYoyoTarget;
        public float amplitude = 2f;        // 上下移动的幅度（距离）
                                            //public float amplitude = 2f;        // 上下移动的幅度（距离）
        public float duration = 1.2f;       // 单程时间（越小越快）
        public Ease easeType = Ease.InOutSine;   // 缓动类型（推荐 InOutSine 最自然）

        private Vector3 startPos;
        private Vector3 startRot;

        private void Start()
        {
            startPos = doYoyoTarget.transform.position;

            startRot = doYoyoTarget.transform.rotation.eulerAngles;
            PlayYoYo();
        }

        private void OnDisable()
        {
            doYoyoTarget.DOKill();
        }


        public void PlayYoYo()
        {
            // 计算目标位置（在初始位置的正上方或下方）
            Vector3 targetPos = startPos + new Vector3(0, amplitude, 0);
            Vector3 targetRot = startRot + new Vector3(0, 180f, 0);



            // 使用 DOMoveY 实现上下移动 + 无限循环
            doYoyoTarget.DOMoveY(targetPos.y, duration)
                .SetEase(easeType)                    // 平滑缓动
                .SetLoops(-1, LoopType.Yoyo);         // -1 = 无限循环，Yoyo = 往返

            doYoyoTarget.DORotate(targetRot, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental);
        }

        public void SceneTransition()
        {
            if (isInSceneTransition)
            {
                StartCoroutine(StartInSceneTransition());
                return;
            }
            StartCoroutine(StartSceneTransition());
        }

        /// <summary>
        /// 跨场景传送
        /// </summary>
        /// <returns></returns>
        IEnumerator StartSceneTransition()
        {
            //保证此脚本对象在跨场景时不被销毁
            DontDestroyOnLoad(gameObject.transform.root);

            Fader fader = FindFirstObjectByType<Fader>();
            //自动保存切换前的场景数据
            SavingWrapper savingWrapper = FindFirstObjectByType<SavingWrapper>();
            savingWrapper.Save();

            Player playerBeforeTransition = Player.GetInstance();
            //禁用角色输入和cc,以防影响角色瞬移
            playerBeforeTransition.DisablePlayerControl();
            playerBeforeTransition.DisableLocomotion();

            //淡出和加载场景
            yield return fader.FadeOut(_fadeOutTime);
            yield return SceneManager.LoadSceneAsync(_loadSceneIndex);

            //在更新角色位置之前,先读取存档数据,使场景更新为上次保存的状态
            savingWrapper.LoadFile();

            //场景切换后寻找新场景的palyer,因为两个场景的player GO不一定是同一个,组件引用可能丢失
            Player playerAfterTransition = Player.GetInstance();
            playerAfterTransition.DisableLocomotion();
            playerAfterTransition.DisablePlayerControl();


            //寻找角色出生点并更新角色位置
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            //保存新场景数据,确保下次角色登录是在新场景
            savingWrapper.Save();

            //尽早恢复cc,使角色恢复落地状态
            playerAfterTransition.EnableLocomotion();

            //等待时间和淡入
            yield return fader.LoadingProgress(_waitTime);
            yield return fader.FadeIn(_fadeInTime);
            //恢复角色输入
            playerAfterTransition.EnablePlayerControl();

            //销毁此脚本对象的根GO
            Destroy(transform.root.gameObject);
        }

        /// <summary>
        /// 场景内传送
        /// </summary>
        /// <returns></returns>
        IEnumerator StartInSceneTransition()
        {
            Fader fader = FindFirstObjectByType<Fader>();

            Player player = GameObject.FindWithTag("Player").GetComponent<Player>();

            //禁用角色输入和cc,以防影响角色瞬移
            player.DisablePlayerControl();
            player.DisableLocomotion();


            //淡出
            yield return fader.FadeOut(_fadeOutTime);

            //加载场景
            // yield return SceneManager.LoadSceneAsync(_loadSceneIndex);

            //寻找角色出生点
            Portal otherPortal = GetOtherPortal();
            UpdatePlayer(otherPortal);

            //等待时间
            yield return new WaitForSeconds(_waitTime);

            //尽早恢复cc,使角色恢复落地状态
            player.EnableLocomotion();

            //淡入
            yield return fader.FadeIn(_fadeInTime);

            //恢复角色输入
            player.EnablePlayerControl();

        }

        public void UpdatePlayer(Portal otherPortal)
        {
            if (otherPortal == null) return;

            var player = Player.GetInstance();
            player.TransTo(otherPortal.spawnPoint);

        }

        private Portal GetOtherPortal()
        {
            Portal[] portals = GameObject.FindObjectsByType<Portal>(FindObjectsSortMode.None);

            foreach (Portal portal in portals)
            {
                if (!object.ReferenceEquals(portal, this) && portal.Destination == Destination)
                {
                    Debug.Log("找到出生点" + portal.gameObject.name + portal.Destination.ToString());
                    return portal;
                }
            }
            Debug.Log("没找到出生点");
            return null;
        }

    }
}
