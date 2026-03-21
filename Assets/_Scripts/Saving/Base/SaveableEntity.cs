using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace RPGDemo.Saving
{
    /// <summary>
    ///每一个SavableEntity代表一个可以被存档的实体，其身上可能挂载有多个ISavable对象，由此Entity统一管理
    /// SaveableEntity应与ISaveable脚本挂在同一GameObject上
    /// </summary>
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
      //CONFIG
      [SerializeField] private string _uniqueIdentifier  = "";
      [SerializeField] private bool useAutoGuid = true;

      public string GetUniqueIdentifier() => _uniqueIdentifier;

      //全局字典，避免使用单例
      private static Dictionary<string, SaveableEntity> globalLookup = new();


      public JToken CapatureState()
      {
          print("SaveableEntity CapatureState");
          JObject state = new JObject();
          IDictionary<string, JToken> stateDict = state;

          foreach (var saveable in GetComponents<ISaveable>())
          {
              JToken token = saveable.CapatureStateAsJToken();
              //直接用类型字符串作为key
              string component = saveable.GetType().ToString();
              stateDict[component] = token;
          }
          return state;

      }

      public void RestoreState(JToken s)
      {
          print("SaveableEntity RestoreStateFromJToken");
          JObject state = s.ToObject<JObject>();
          IDictionary<string, JToken> stateDict = state;

          foreach (var saveable in GetComponents<ISaveable>())
          {
              string typeString = saveable.GetType().ToString();
              if (stateDict.ContainsKey(typeString))
              {
                  saveable.RestoreStateFromJToken(stateDict[typeString]);
              }
          }
      }

#if UNITY_EDITOR
        //ExecuteAlways，在编辑器模式下也能执行，用于生成GUID
        private void Update()
        {
            if(!useAutoGuid) return;
            //若当前物体在PlayMode里，返回
            if(Application.IsPlaying(gameObject)) return;
            //若当前物体不在任何场景中，返回（防止作为预制体时运行后面的代码）
            if(string.IsNullOrEmpty(gameObject.scene.path)) return;

            //通过反射赋值GUID
            //对于IDispose对象，使用完后要手动释放避免内存泄漏，C#提供using方法来自动释放IDispose对象
            using (SerializedObject serializedObject = new SerializedObject(this))
            {
                using (SerializedProperty property = serializedObject.FindProperty("_uniqueIdentifier"))
                {
                    if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
                    {
                        //对符合条件的对象生成新的GUID并赋值
                        property.stringValue = System.Guid.NewGuid().ToString();
                        serializedObject.ApplyModifiedProperties();
                    }

                    globalLookup[property.stringValue] = this;
                }
            }

        }
#endif


        /// <summary>
        /// 判断是否为唯一ID
        /// </summary>
        /// <param name="candidate"></param>
        /// <returns></returns>
        private bool IsUnique(string candidate)
        {
            //字典里没有记录，说明是唯一
            if (!globalLookup.ContainsKey(candidate)) return true;

            //字典记录的等于当前对象，说明是唯一
            if (globalLookup[candidate] == this) return true;

            //字典记录的为null,说明是唯一，移除key
            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            //字典记录的不等于当前对象，移除key后是唯一
            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            //若都不满足，说明不是唯一
            return false;
        }

    }
}
