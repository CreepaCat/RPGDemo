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
            var animatorHandler = caster.GetComponent<PlayerAnimatorHandler>();
            if (animatorHandler.IsInteracting || animatorHandler.IsHandInteracting) return false;
            return true;

            //  if (isHandInteractingAnima && animatorHandler.IsInteracting || animatorHandler.IsHandInteracting) return false;
        }

        public override void PlayAnimation(Character caster)
        {
            Debug.Log(caster + "播放施法动画片段" + animationClip.name);
            // caster.GetComponent<Animator>().CrossFade(animationClip.name, 0.2f);
            var animatorHandler = caster.GetComponent<PlayerAnimatorHandler>();
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
