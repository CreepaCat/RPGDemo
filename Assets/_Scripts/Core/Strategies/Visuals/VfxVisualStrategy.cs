using System.Collections.Generic;
using RPGDemo.Core.Strategies;
using UnityEngine;

public class VfxVisualStrategy : VisualStrategy
{
    public ParticleSystem vfx;
    public Vector3 spawnOffset;
    public float destroyAfterSeconds = 3f;
    public override void Play(Character caster, List<Character> targets, string skillId, Transform castPoint = null)
    {
        Transform spawnPoint = castPoint != null ? castPoint : caster.transform;
        var vfxGo = GameObject.Instantiate(vfx, spawnPoint.position + spawnOffset, vfx.transform.rotation);
        DestroyAfterEffect das = vfxGo.GetComponent<DestroyAfterEffect>();
        if (das != null)
        {
            das.SetDestroyDelay(destroyAfterSeconds);
        }
    }
}
