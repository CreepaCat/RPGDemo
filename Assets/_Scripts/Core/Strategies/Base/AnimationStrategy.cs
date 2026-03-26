using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    [System.Serializable]
    public abstract class AnimationStrategy : IAnimationStrategy
    {
        [field: SerializeField] public float DelayTime { get; private set; }

        [field: SerializeField] public AnimationClip animationClip { get; private set; }

        public abstract bool CanPlayAnimation(Character caster);

        public abstract void PlayAnimation(Character caster);



    }
}
