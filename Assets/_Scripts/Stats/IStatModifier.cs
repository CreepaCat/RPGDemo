using System.Collections.Generic;
using UnityEngine;
namespace RPGDemo.Stats
{
    /// <summary>
    /// 定义附加数值修改这,如装备数值,buff数值对角色最终数值的影响
    /// </summary>
    public interface IStatModifierProvider
    {
        //返回值为IEnumerable是因为一个脚本当中可能有多个数值影响
        //比如某个buff同时影响移动速度和恢复速度
        IEnumerable<float> GetAdditiveModifiers(StatsType statsType);

        IEnumerable<float> GetPercentageModifiers(StatsType statsType);
    }
}
