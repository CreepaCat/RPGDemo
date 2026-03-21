using UnityEngine;

public class PersistanceObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject persistanceObjectPrefab;
   // [SerializeField] GameObject disenableBeforeCreatePersistanceObject;
    
    private static bool _hasSpawned = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void OnEnterPlayMode()
    {
        _hasSpawned = false;
    }

    private void Awake()
    {
       // disenableBeforeCreatePersistanceObject.SetActive(false);
        if(_hasSpawned) return;
        SpawnPersistanceObject();
        _hasSpawned = true;
    }

    void SpawnPersistanceObject()
    {
        Debug.Log("Spawning persistance object...");
        GameObject persistanceObject = Instantiate(persistanceObjectPrefab);
        DontDestroyOnLoad(persistanceObject);
    }
}
