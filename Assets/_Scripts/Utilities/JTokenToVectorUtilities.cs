using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public static class JTokenToVectorUtilities
{
  public static JToken ToJToken(this Vector3 vector)
  {
    JObject state = new JObject();
    IDictionary<string, JToken> stateDict = state;
    stateDict["x"] = vector.x;
    stateDict["y"] = vector.y;
    stateDict["z"] = vector.z;
    return state;
  }

  public static Vector3 ToVector3(this JToken state)
  {
    Vector3 result = new Vector3();
    //里氏替换
    if (state is JObject jObject)
    {
      IDictionary<string, JToken> stateDict = jObject;
      if (stateDict.ContainsKey("x"))
      {
        result.x = stateDict["x"].Value<float>();
      }

      if (stateDict.ContainsKey("y"))
      {
        result.y = stateDict["y"].Value<float>();
      }

      if (stateDict.ContainsKey("z"))
      {
        result.z = stateDict["z"].Value<float>();
      }
    }
    
    return result;
  }
}
