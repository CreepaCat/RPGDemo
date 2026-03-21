using UnityEngine;

public class AnimatorBoolResetor : StateMachineBehaviour
{
   [SerializeField] private string animBoolName;

   [SerializeField] private bool status;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    { 
        animator.SetBool(animBoolName,status);
    }
}
