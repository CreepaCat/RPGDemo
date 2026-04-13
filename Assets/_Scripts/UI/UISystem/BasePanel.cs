using UnityEngine;

namespace RPGDemo.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanel : MonoBehaviour
    {

        protected CanvasGroup canvasGroup;
        protected virtual void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        /// <summary>
        /// 面板显示时调用
        /// </summary>
        public virtual void OnShow()
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

        }

        /// <summary>
        /// 面板隐藏时调用
        /// </summary>
        public virtual void OnHide()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// 关闭当前面板（推荐在面板内部按钮调用）
        /// </summary>
        protected void CloseSelf()
        {
            UIManager.Instance.ClosePanel();
        }
    }
}
