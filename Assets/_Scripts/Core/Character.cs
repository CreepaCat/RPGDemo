using System;
using RPGDemo.Combat;
using RPGDemo.Inventories.ActionBar;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Fighter TargetingSystem { get; internal set; }
    public object CastPoint { get; internal set; } //投射物射出点
}
