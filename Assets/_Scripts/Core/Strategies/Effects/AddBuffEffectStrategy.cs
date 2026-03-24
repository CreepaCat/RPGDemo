using RPGDemo.Buffs;
using RPGDemo.Core.Strategies;
using UnityEngine;

public class AddBuffEffectStrategy : EffectStrategy
{
    public BuffInstance buff;
    // public float buffValue;
    public override void Apply(Character caster, Character target, string skillId)
    {

        buff.Setup(caster, target);
        target.GetComponent<BuffStore>().ApplyBuff(buff, caster, target, buff.data.baseDuration);
    }
}
