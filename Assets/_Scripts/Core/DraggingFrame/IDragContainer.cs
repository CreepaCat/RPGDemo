namespace RPGDemo.Core.DraggingFrame
{
    /// <summary>
    /// 用拖入接口和拖出接口来定义一个可拖动物品容器接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDragContainer<T> : IDragSource<T>, IDragDestination<T> where T : class
    {

    }
}
