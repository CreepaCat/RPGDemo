using System.Collections;
using System.Collections.Generic;
using Core.AudioSystem;
using RPGDemo.Stats;
using RPGDemo.Weapons;
using UnityEngine;

namespace RPGDemo.Combat
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("攻击粒子特效播放偏移量")]
        [SerializeField] Vector3 posOffset;
        [SerializeField] Quaternion rotateOffset;

        [Header("对象Layer")]
        [SerializeField] LayerMask damageLayer;

        public Transform owner;

        HashSet<CombatTarget> _targets = new HashSet<CombatTarget>();

        Collider damageCollider;
        Weapon weapon;

        Coroutine playingAttackVfx;
        ParticleSystem ps;



        private void Awake()
        {
            damageCollider = GetComponent<Collider>();
            damageCollider.isTrigger = true;
            DisableCollider();
            ps = GetComponentInChildren<ParticleSystem>();

        }


        public void Setup(Transform owner, LayerMask damageLayer, Weapon weapon)
        {

            this.owner = owner;
            this.damageLayer = damageLayer;
            this.weapon = weapon;

        }

        public void PlayAttackVfx()
        {

            if (ps == null) return;
            if (playingAttackVfx != null)
            {
                StopCoroutine(playingAttackVfx);
                ps.transform.SetParent(transform);
            }
            playingAttackVfx = StartCoroutine(PlayingAttackVfxCoroutine(ps));


        }

        IEnumerator PlayingAttackVfxCoroutine(ParticleSystem ps)
        {

            ps.transform.SetParent(null);
            ps.transform.position = transform.position + posOffset;
            ps.transform.rotation = transform.rotation * rotateOffset;
            ps.Play();
            while (ps.isPlaying)
            {
                yield return null;
            }
            ps.transform.SetParent(transform);

        }

        public void EnableCollider()
        {
            _targets.Clear();
            damageCollider.enabled = true;
        }

        public void DisableCollider()
        {
            damageCollider.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.transform == owner) return;
            if (((1 << other.gameObject.layer) & damageLayer.value) == 0)
            {
                return;
            }
            if (other.TryGetComponent(out CombatTarget target) && !_targets.Contains(target))
            {
                _targets.Add(target);

                target.TakeDamage(owner.GetComponent<BaseStats>().GetStats(StatsType.Attack));


                var sound = weapon.CurrentWeaponConfig.HitSound;
                if (sound == null) return;
                SoundManager.Instance.CreateSound()
                .WithSound(sound)
                .WithPlayPosition(target.transform.position)
                .WithRandomPitch()
                .Play();


            }
        }
    }
}
