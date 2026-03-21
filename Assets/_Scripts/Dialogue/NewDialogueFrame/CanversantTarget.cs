using UnityEngine;

namespace NewDialogueFrame
{
    /// <summary>
    /// 定义一个可对话对象，并可用于触发事件
    /// </summary>
    public class CanversantTarget:MonoBehaviour
    {
        public void OnPlayerAccept()
        {
            Debug.Log("CanversantTarget.OnPlayerAccept:给与玩家任务道具");
        }
        public void OnPlayerReject()
        {
            Debug.Log("CanversantTarget.OnPlayerReject:玩家拒绝了矮人的请求，好感度-10");
        }
    }
}