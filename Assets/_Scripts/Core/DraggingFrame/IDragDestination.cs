namespace RPGDemo.Core.DraggingFrame
{
    /// <summary>
    /// 物品拖入格
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDragDestination<T> where T : class
    {

        /// <summary>
        /// 获取当前容器最大接收数（最大容量 - 当前物品数）
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int GetMaxAcceptable(T item);

        void AddItems(T item, int amount);
    }
}
