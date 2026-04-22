using UnityEngine;
namespace RPGDemo.Core.Strategies
{

    [System.Serializable]

    public class CastingAnimationStrategy : AnimationStrategy
    {
        public bool isInteractingAnima = true;
        public bool isHandInteractingAnima = false;

        public override bool CanPlayAnimation(Character caster)
        {
            var animatorHandler = caster.GetComponent<PlayerAnimationHandler>();
            if (animatorHandler.IsInteracting || animatorHandler.IsHandInteracting) return false;
            return true;
        }

        public override void PlayAnimation(Character caster)
        {
            Debug.Log(caster + "播放施法动画片段" + animationClip.name);
            var animatorHandler = caster.GetComponent<PlayerAnimationHandler>();
            if (isInteractingAnima)
            {
                animatorHandler.PlayTargetAnimation(Animator.StringToHash(animationClip.name), true);

            }
            else if (isHandInteractingAnima)
            {
                animatorHandler.PlayTargetHandMaskedAnimation(Animator.StringToHash(animationClip.name), true);
            }

            animatorHandler.PlayTargetAnimation(Animator.StringToHash(animationClip.name), false);
            caster.GetComponent<Animator>().SetBool("isCasting", true);


        }
    }
}
