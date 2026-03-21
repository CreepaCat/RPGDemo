using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Saving;
using UnityEngine;

[RequireComponent(typeof(SaveableEntity))]
public class SaveableCube : MonoBehaviour,ISaveable
{
    [SerializeField ]private GameObject child;
    
    [SerializeField]bool isChildActive = false;

    private void Start()
    {
       ApplayChildState();
    }

    private void ApplayChildState()
    {
        child.SetActive(isChildActive);
    }
    
   
    //直接用枚举作为存档key
    private enum ChildeStateSaveDate
    {
        ChildActive,
    }

   
     JToken ISaveable.CapatureStateAsJToken()
    {
        Debug.Log("SaveableCube CapatureStateAsJToken");
        JObject state = new JObject();
        IDictionary<string, JToken> stateDice = state;
        
        string key = ChildeStateSaveDate.ChildActive.ToString();
        stateDice[key] = JToken.FromObject(isChildActive);
        return state;
    }

    void ISaveable.RestoreStateFromJToken(JToken s)
    {
        Debug.Log("SaveableCube RestoreStateFromJToken");
        //throw new System.NotImplementedException();
        JObject state = s as JObject;
        IDictionary<string, JToken> stateDict = state;
        
        string key = ChildeStateSaveDate.ChildActive.ToString();
        if (stateDict.ContainsKey(key))
        {
            this.isChildActive = stateDict[key].ToObject<bool>();
        }
        //在每次读取存档后将数据应用到对象
        ApplayChildState();
    }
    
    
}
