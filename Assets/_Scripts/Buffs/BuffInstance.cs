using RPGDemo.Attributes;
using RPGDemo.Stats;

namespace RPGDemo.Buffs
{
    /// <summary>
    /// 运行时buff实例类
    /// </summary>
    public class BuffInstance
    {
        public BuffSO data;
        public float remainingTime;
        public int currentStack = 1; //buff层数
        public Character owner; // 施加者（可用于归属伤害）
        public Character target;
        public float nextTickTime;        // 下次 tick 时间戳


        public void ApplyEffects()
        {
            if (data.isDamageOrHeal)
            {
                if (data.healValue > 0f) //治疗
                {
                    target.GetComponent<Health>().Heal(data.healValue);
                }

                if (data.damageValue > 0f)
                {
                    //todo：buff伤害同样要计算发出者的攻击力和目标的防御数值
                    target.GetComponent<Health>().TakeDamage(data.damageValue);
                }
            }
        }

    }
}
