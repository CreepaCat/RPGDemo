using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    /// <summary>
    /// 作用范围策略
    /// </summary>
    public interface IRangeStrategy
    {
        float MaxCastDistance { get; }
        bool IsPositionInRange(Vector3 casterPos, Vector3 worldPos);
        // AoE 用：返回范围内所有可检测点（用于后续过滤）
        List<Vector3> GetAreaPositions(Character caster);
    }
}
