using System.Collections.Generic;
using RPGDemo.Core.Strategies;
using UnityEngine;

/// <summary>
/// 圆形范围选择策略
/// </summary>
[CreateAssetMenu(menuName = "RPGDemo/Strategy/Range/CircleRangeStrategy")]
public class CircleRangeStrategy : RangeStrategy
{
    public float raidius = 10f;
    //todo:角度限制，是否在前方扇形范围
    public override List<Vector3> GetAreaPositions(Character caster)
    {

        // throw new System.NotImplementedException();
        return null;
    }

    public override bool IsPositionInRange(Vector3 casterPos, Vector3 worldPos)
    {
        if (Vector3.Distance(casterPos, worldPos) < MaxCastDistance)
        {
            return true;
        }
        return false;
    }
}
