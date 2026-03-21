namespace RPGDemo.Inventories.Utils
{
    public interface IItemGetter
    {
        InventoryItem GetItem();
        int GetAmount();
    }
}