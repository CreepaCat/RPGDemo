using System;

namespace RPGDemo
{
    /// <summary>
    /// 懒初始化
    /// </summary>
    public interface ILazyInit
    {
        bool IsInitialized { get; }
        void LazyInit(Action onDone = null);
    }
}
