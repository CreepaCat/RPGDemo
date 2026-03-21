using RPGDemo.Core.Strategies;
using UnityEngine;
namespace RPGDemo.Skills
{
    [RequireComponent(typeof(Rigidbody))] // 或不使用物理，用 transform 手动移动
    public class ProjectileMover : MonoBehaviour
    {
        public IProjectileStrategy strategy;     // 在生成时注入
        public Character owner;
        public float lifetime = 10f;
        private float spawnTime;

        private void Awake()
        {
            spawnTime = Time.time;
        }

        public void Launch(IProjectileStrategy strat, Character caster, Character target, Vector3 startPos, Vector3 dir)
        {
            owner = caster;
            strategy = strat;
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
                // 示例：对目标应用效果（如果技能有多个 EffectStrategy）
                // 这里可以从 Skill 传过来的效果列表应用
                Destroy(gameObject);
            }
        }

    }
}
