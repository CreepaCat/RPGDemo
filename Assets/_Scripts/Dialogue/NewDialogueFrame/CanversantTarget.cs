using System;
using RPGDemo.Quests;
using UnityEngine;

namespace NewDialogueFrame
{
    /// <summary>
    /// 定义一个可对话对象，并可用于触发事件
    /// </summary>
    [RequireComponent(typeof(AIQuestHandler))]
    public class CanversantTarget : MonoBehaviour
    {

        public event Action OnDialogueStart;

        public void StartDialogue()
        {
            OnDialogueStart?.Invoke();
            GetComponentInParent<NPC>().AnimationHandler.PlayTargetAnimation(Animator.StringToHash("Talk"), true);
        }
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
