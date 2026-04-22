using System;
using RPGDemo.Core.Strategies;
using UnityEngine;

namespace RPGDemo.Projectiles
{
    [RequireComponent(typeof(Rigidbody), typeof(ParticleSystem), typeof(AudioSource))] // 或不使用物理，用 transform 手动移动
    public class ProjectileController : MonoBehaviour
    {

        public ParticleSystem hitVfx;
        public AudioClip hitSfx;
        public IProjectileStrategy strategy;     // 在生成时注入
        public string skillId;  //生成时注入，技能缓存 todo:或许可以从资源文件夹实时加载skill,只需记录ID即可
        public Character owner;
        public float lifetime = 10f;
        private float spawnTime;

        Rigidbody rb;
        Transform targetTransform = null;
        AudioSource audioSource;

        Action<Character> hitAction;

        private void Awake()
        {
            spawnTime = Time.time;
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.freezeRotation = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 高速投射物

            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 1f;  //3d空间音效

            GetComponent<Collider>().isTrigger = true;
        }

        public void Launch(IProjectileStrategy strat, Character caster, Character target,
                            string skillGuid, Vector3 launchPos, Vector3 dir)
        {
            transform.rotation = Quaternion.LookRotation(dir);
            owner = caster;
            targetTransform = target?.transform;
            strategy = strat;
            skillId = skillGuid;
            strategy.Initialize(gameObject, caster, target, launchPos, dir);

            Destroy(gameObject, lifetime);
        }

        private void FixedUpdate()
        {

            strategy.UpdateProjectile(rb, targetTransform, Time.fixedDeltaTime);


        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform == owner.transform) return;
            if (other.isTrigger) return;

            //对于必中弹
            if (targetTransform != null && other.transform != targetTransform) return;


            // 碰撞逻辑：伤害、特效、销毁等
            // 可通过 owner 或 event 或回调方法 调用 EffectStrategies
            //说明是直线发射无目标飞弹，直接对第一个命中物生效
            if (other.transform.TryGetComponent<Character>(out var target))
            {
                Debug.Log(owner + "投射物命中目标" + target);
                hitAction?.Invoke(target);
            }

            var hitpos = other.bounds.ClosestPoint(transform.position);
            audioSource.PlayOneShot(hitSfx);

            var hitNormal = (owner.transform.position - hitpos).normalized;
            //根据碰撞点的法线来决定方向
            Instantiate(hitVfx, hitpos, Quaternion.LookRotation(hitNormal));
            Destroy(gameObject);



        }

        /// <summary>
        /// 投射物命中后的回调函数
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetCallback(Action<Character> callback)
        {
            hitAction += callback;

        }
    }
}
