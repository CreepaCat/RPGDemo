using System;
using System.Diagnostics;
using RPGDemo.Combat;
using RPGDemo.Inventories.ActionBar;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum CastType
    {
        None,
        RightHand,
        LeftHand,
        Weapon,
        Head,
    }
    public Fighter TargetingSystem { get; internal set; }
    public Transform RightHand;
    public Transform LeftHand;
    public Transform WeaponHolder;
    public Transform Head;

    public Transform GetCastTransform(CastType castType)
    {
        switch (castType)
        {
            case CastType.RightHand:
                return RightHand;
            case CastType.LeftHand:
                return LeftHand;
            case CastType.Weapon:
                return WeaponHolder;
            case CastType.Head:
                return Head;
            default:
                return transform;
        }
    }
}
