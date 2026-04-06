using System;
using MyBehaviourTree;
using UnityEngine;

public class IsPlayerInRange : ICondition
{
    public float range = 1f;

    public bool Check(BehaviourTreeContext context)
    {

        return Vector3.Distance(context.Transform.position, context.Blackboard.GetPlayer().position) < range;

    }

}
