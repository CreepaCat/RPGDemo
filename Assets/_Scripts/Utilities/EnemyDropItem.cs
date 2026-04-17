using System.Collections.Generic;
using RPGDemo.Inventories;
using RPGDemo.Inventories.Utils;
using UnityEngine;

[RequireComponent(typeof(ItemDropper))]
public class EnemyDropItem : MonoBehaviour
{
    [SerializeField] InventoryItem itemsCofig;
    [SerializeField] int amountToDrop = 1;

    Enemy enemy;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        enemy.Health.OnDeath += DropItem;
    }

    private void OnDestroy()
    {

        enemy.Health.OnDeath -= DropItem;
    }

    private void DropItem()
    {
        if (itemsCofig == null) return;
        var dropper = GetComponent<ItemDropper>();
        dropper.DropItem(itemsCofig, amountToDrop);
    }


}
