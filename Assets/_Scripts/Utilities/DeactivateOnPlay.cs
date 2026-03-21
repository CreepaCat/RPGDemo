using UnityEditor;
using UnityEngine;

[InitializeOnLoad]  // 确保脚本在 Unity 启动时自动加载
public class DeactivateOnPlay
{
    const string TargetName = "DeactivateOnPlay";
    static DeactivateOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
     
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            // 在进入 Play Mode 前执行禁用操作
            GameObject target = GameObject.Find(TargetName);  // 替换为你的 GameObject 名称
            if (target != null)
            {
                target.SetActive(false);
                Debug.Log("Deactivated: " + target.name);
            }
            else
            {
                Debug.LogWarning("GameObject not found!");
            }
        }
        
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj.name == TargetName && obj.scene.isLoaded && !obj.activeSelf)  // 匹配名称、当前场景、且 inactive
                {
                    obj.SetActive(true);
                    Debug.Log("Activated: " + obj.name);
                    return;  // 如果只有一个匹配，找到后退出循环
                }
            }
        }
        
    }
    
   
}