using RPGDemo.Inventories;
using RPGDemo.Inventories.Utils;
using UnityEngine;

public abstract class SlotUI : MonoBehaviour, IItemGetter
{
    public abstract int GetAmount();

    public abstract InventoryItem GetItem();

}
