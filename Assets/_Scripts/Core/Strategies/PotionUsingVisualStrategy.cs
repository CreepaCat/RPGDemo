using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 血量要求策略
    /// </summary>
    //[CreateAssetMenu(menuName = "RPGDemo/Strategy/Visual/PotionUsingVisual")]
    public class PotionUsingVisualStrategy : VisualStrategy
    {
        public ParticleSystem onUsingVfxPrefab;
        public float destroyAfterSeconds = 1;
        public override void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null)
        {
            ParticleSystem vfx = GameObject.Instantiate(onUsingVfxPrefab, caster.transform);
            vfx.transform.position = caster.transform.position + Vector3.up;
            //Destroy(vfx.gameObject, destroyAfterSeconds);

        }
    }
}
