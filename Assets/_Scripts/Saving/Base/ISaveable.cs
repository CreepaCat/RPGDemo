using Newtonsoft.Json.Linq;
using UnityEngine;

namespace RPGDemo.Saving
{

    public interface ISaveable
    {
        JToken CapatureStateAsJToken();
        void RestoreStateFromJToken(JToken s);
        
    }
}

