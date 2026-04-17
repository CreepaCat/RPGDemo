using UnityEngine;

public class GameStatus : MonoBehaviour
{
    public static GameStatus Instance { get; private set; }

    [SerializeField] GameObject console = null;
    public bool IsUsingConsole => console != null && console.activeSelf;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
    }


}
