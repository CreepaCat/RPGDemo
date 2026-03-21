using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPGDemo
{
    public interface ILazyInit
    {
        bool IsInitialized { get; }
        void LazyInit(Action onDone = null);
    }
}
