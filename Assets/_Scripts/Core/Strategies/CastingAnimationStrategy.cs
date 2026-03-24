using UnityEngine;
namespace RPGDemo.Core.Strategies
{

    public class CastingAnimationStrategy : AnimationStrategy
    {
        public override void PlayAnimation(Character caster)
        {
            Debug.Log(caster + "播放动画片段" + animationClip.name);
            // caster.GetComponent<Animator>().CrossFade(animationClip.name, 0.2f);
            caster.GetComponent<PlayerAnimatorHandler>().PlayTargetAnimation(Animator.StringToHash(animationClip.name), true);

        }
    }
}
