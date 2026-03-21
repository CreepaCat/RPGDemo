namespace RPGDemo.Core.DraggingFrame
{
    /// <summary>
    /// 物品拖出格
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDragSource<T> where T : class
    {
        T GetItem();  //获取此格中的Item
        int GetAmount(); //获取此格中该Item数量

        void RemoveItems(int amount); //从此格中移除该数量的Item
    }
}
