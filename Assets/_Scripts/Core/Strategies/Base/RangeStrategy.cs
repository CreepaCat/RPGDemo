using System.Collections.Generic;
using UnityEngine;

namespace RPGDemo.Core.Strategies
{
    public abstract class RangeStrategy : ScriptableObject, IRangeStrategy
    {
        public float MaxCastDistance { get; set; }

        public abstract List<Vector3> GetAreaPositions(Character caster);

        public abstract bool IsPositionInRange(Vector3 casterPos, Vector3 worldPos);

    }
}
