using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    public interface IAnimationStrategy
    {
        float DelayTime { get; }

        AnimationClip animationClip { get; }

        void PlayAnimation(Character caster);
        bool CanPlayAnimation(Character caster);

    }
}
