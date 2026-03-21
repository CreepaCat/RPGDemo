using System.Collections.Generic;
using RPGDemo.Core.Strategies;
using RPGDemo.Skills;
using UnityEngine;

namespace RPGDemo.Projectiles
{
    // Assets/Projectiles/ProjectileController.cs
    [RequireComponent(typeof(Rigidbody))] // 或不使用物理，用 transform 手动移动
    public class ProjectileController : MonoBehaviour
    {
        public IProjectileStrategy strategy;     // 在生成时注入
        public string skillId;  //生成时注入，技能缓存 todo:或许可以从资源文件夹实时加载skill,只需记录ID即可
        public Character owner;
        public float lifetime = 10f;
        private float spawnTime;

        private void Awake()
        {
            spawnTime = Time.time;
        }

        public void Launch(IProjectileStrategy strat, Character caster, Character target,
                            string skillGuid, Vector3 startPos, Vector3 dir)
        {
            owner = caster;
            strategy = strat;
            skillId = skillGuid;
            strategy.InitializeAndLaunch(this.gameObject, caster, target, startPos, dir);

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            if (strategy != null && strategy.RequiresUpdate)
            {
                // strategy 可以在这里继续控制（追踪类常用）
                // 或让 strategy 自己接管 Update（见下方实现）
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 碰撞逻辑：伤害、特效、销毁等
            // 可通过 owner 或 event 调用 EffectStrategies
            if (other.TryGetComponent<Character>(out var target) && target != owner)
            {
                Debug.Log(owner + "投射物命中目标" + target);


                // 示例：对目标应用效果（如果技能有多个 EffectStrategy）
                // 这里可以从 Skill 传过来的效果列表应用
                SkillItem skill = SkillItem.GetItemFromID(skillId) as SkillItem;
                foreach (var effect in skill.effectStrategies)
                {
                    effect.Apply(owner, target, skillId);
                }
                Destroy(gameObject);
            }
        }
    }
}
