using System.Collections.Generic;
using RPGDemo.Stats;
using UnityEngine;

namespace RPGDemo.Inventories
{
    /// <summary>
    /// MVC Model
    /// </summary>

    public abstract class EquipableItem : InventoryItem
    {
        [SerializeField] private EquipLocation _allowedEquipLocation;
        [SerializeField] int _levelLimitation = 1;

        [Header("装备数值")]

        [Header("固定数值")]
        [SerializeField] private List<StatBonus> additiveBonuses;

        [Header("百分比数值")]
        [SerializeField] private List<StatBonus> percentageBonuses;


        public IEnumerable<float> GetAdditiveModifiers(StatsType type)
        {
            foreach (var bonus in additiveBonuses)
            {
                if (bonus.statsType == type)
                {
                    yield return bonus.value;
                }
            }
        }
        public IEnumerable<float> GetPercentageModifiers(StatsType type)
        {
            foreach (var bonus in percentageBonuses)
            {
                if (bonus.statsType == type)
                {
                    yield return bonus.value;
                }
            }
        }


        public EquipLocation GetAllowedEquipLocation() => _allowedEquipLocation;
        public int GetLevelLimitation() => _levelLimitation;




    }
}
