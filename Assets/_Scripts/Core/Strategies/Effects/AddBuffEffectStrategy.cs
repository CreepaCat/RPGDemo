using RPGDemo.Buffs;
using RPGDemo.Core.Strategies;
using UnityEngine;

[System.Serializable]

public class AddBuffEffectStrategy : EffectStrategy
{
    public BuffInstance buff;
    // public float buffValue;
    public override void Apply(Character caster, Character target, string skillId)
    {

        target.GetComponent<BuffStore>().ApplyBuff(buff, caster, target, buff.data.baseDuration);
    }
}
