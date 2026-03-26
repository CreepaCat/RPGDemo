using RPGDemo.Buffs;
using UnityEngine;

public class BuffStoreUI : MonoBehaviour
{
    [SerializeField] BuffSlotUI buffSlotPrefab = null;
    [SerializeField] Transform buffSlotRoot = null;

    BuffStore buffStore;

    private void Awake()
    {
        buffStore = GetComponentInParent<BuffStore>();
    }

    private void OnEnable()
    {
        buffStore.OnStoreUpdated += DrawUI;
    }

    private void OnDisable()
    {
        buffStore.OnStoreUpdated -= DrawUI;

    }

    private void Start()
    {
        DrawUI();

    }


    void DrawUI()
    {
        foreach (Transform child in buffSlotRoot)
        {
            Destroy(child.gameObject);
        }

        foreach (var buffInstance in buffStore.GetActiveBuffs())
        {
            BuffSlotUI buffSlotUI = Instantiate(buffSlotPrefab, buffSlotRoot);
            buffSlotUI.Setup(buffInstance);
        }
    }
}
