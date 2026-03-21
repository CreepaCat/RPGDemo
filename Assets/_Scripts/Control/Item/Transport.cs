using UnityEngine;

public class Transport : MonoBehaviour
{
    [SerializeField] private GameObject transportStone;
    private bool isOn = false;

    public void Toggle()
    {
        isOn = !isOn;
        transportStone.SetActive(isOn);
    }
    
}
