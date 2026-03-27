using UnityEngine;

public class AnimatorBoolResetor : StateMachineBehaviour
{

    [System.Serializable]
    public struct AnimBoolInfo
    {
        public string animBoolName;
        public bool status;
    }
    [SerializeField] AnimBoolInfo[] animBoolInfos;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        for (int i = 0; i < animBoolInfos.Length; i++)
        {
            animator.SetBool(animBoolInfos[i].animBoolName, animBoolInfos[i].status);
        }
    }
}
