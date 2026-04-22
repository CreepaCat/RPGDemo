using UnityEngine;
/// <summary>
/// 持久化GameObject生成模式,避免使用Mono单例
/// </summary>
public class PersistanceObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistanceObjectPrefab;

    private static bool _hasSpawned = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void OnEnterPlayMode()
    {
        _hasSpawned = false;
    }

    private void Awake()
    {
        if (_hasSpawned) return;
        SpawnPersistanceObject();
        _hasSpawned = true;
    }

    void SpawnPersistanceObject()
    {
        GameObject persistanceObject = Instantiate(persistanceObjectPrefab);
        DontDestroyOnLoad(persistanceObject);
    }
}
