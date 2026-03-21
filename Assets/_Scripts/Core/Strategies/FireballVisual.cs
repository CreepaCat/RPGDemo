using System.Collections.Generic;
using RPGDemo.Projectiles;
using RPGDemo.Skills;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{

    /// <summary>
    /// 火球投射物特效策略
    /// </summary>
    [CreateAssetMenu(menuName = "RPGDemo/Strategy/ProjectileVisual/FireballVisual")]

    public class FireballVisual : ProjectileVisualStrategy
    {

        public override void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null)
        {
            if (projectilePrefab == null || projectileStrategy == null) return;
            Character primary = null;
            if (targets != null && targets.Count > 0)
            {
                primary = targets[0]; //选择第一个目标
            }

            //如果没有发射点就从caster身上发出
            Vector3 launchPos = castPoint
            ? castPoint.position
            : caster.transform.position + Vector3.up;

            //如果没有目标，就直线发射
            Vector3 dir = primary
            ? (primary.transform.position - launchPos).normalized
            : caster.transform.forward;

            var proj = Instantiate(projectilePrefab, launchPos, Quaternion.identity);
            var controller = proj.GetComponent<ProjectileController>();
            if (controller)
            {
                controller.Launch(projectileStrategy, caster, primary, skillId, launchPos, dir);
            }

        }
    }
}
