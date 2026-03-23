using System.Collections.Generic;
using RPGDemo.Core.Strategies;
using RPGDemo.Projectiles;
using UnityEngine;

/// <summary>
/// 火球投射物特效策略
/// </summary>
[CreateAssetMenu(menuName = "RPGDemo/Strategy/ProjectileVisual/MultiFirebal")]
public class MultiFirebalVisual : ProjectileVisualStrategy
{
    public override void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null)
    {
        if (projectilePrefab == null || projectileStrategy == null) return;
        // Character primary = null;

        //如果没有发射点就从caster身上发出
        Vector3 launchPos = castPoint == null
        ? caster.transform.position + Vector3.up * 2f
        : castPoint.position;


        foreach (var target in targets)
        {
            Debug.Log("MultiFirebalVisual 目标:" + target?.transform.name);
            if (target == null) return;

            //如果没有目标，就直线发射
            Vector3 dir = target != null
            ? (target.transform.position - launchPos).normalized
            : caster.transform.forward;

            var proj = Instantiate(projectilePrefab, launchPos, Quaternion.identity);
            var controller = proj.GetComponent<ProjectileController>();
            if (controller)
            {
                controller.Launch(projectileStrategy, caster, target, skillId, launchPos, dir);
            }

        }


    }
}
