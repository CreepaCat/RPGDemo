using UnityEngine;

public class RestHandInteractingParam : StateMachineBehaviour
{
  
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(PlayerAnimatorParamConfig.animIDIsHandInteracting, false);
        // FindFirstObjectByType<Player>().AnimatorHandler.ResetInteractingFlag();
    }
}
