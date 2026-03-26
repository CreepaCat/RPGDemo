using RPGDemo.Attributes;
using RPGDemo.Core.Strategies;
using RPGDemo.Inventories.ActionBar;
using UnityEngine;

/// <summary>
/// 恢复法力效果策略
/// </summary>
//[CreateAssetMenu(menuName = "RPGDemo/Strategy/Effect/RestoreMana")]
[System.Serializable]

public class RestoreManaEffect : EffectStrategy
{
    public float restoreValue = 10f;
    public override void Apply(Character caster, Character target, string itemId)
    {
        // PotionItem potion = PotionItem.GetItemFromID(itemId) as PotionItem;
        // if (potion != null && potion.useValue > 0f)
        // {

        //     restoreValue = potion.useValue;
        // }
        Debug.Log("RestoreManaEffect" + restoreValue);

        target.GetComponent<Mana>()?.RecoverMana(restoreValue);

    }
}
