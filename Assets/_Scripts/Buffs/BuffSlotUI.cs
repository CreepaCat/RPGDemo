using RPGDemo.Buffs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffSlotUI : MonoBehaviour
{
    [SerializeField] Image buffIcon = null;
    [SerializeField] TextMeshProUGUI tmp_buffStackNum = null;
    [SerializeField] Image img_remainingTimeCover = null;
    BuffInstance buffInstance;
    public void Setup(BuffInstance buffInstance)
    {

        this.buffInstance = buffInstance;
        buffIcon.sprite = buffInstance.data.icon;
        DrawUI();
    }

    private void Update()
    {
        DrawUI();

    }

    private void DrawUI()
    {
        if (buffInstance.data.isStackable)
        {
            tmp_buffStackNum.text = buffInstance.currentStack.ToString();
        }
        else
        {
            tmp_buffStackNum.gameObject.SetActive(false);
        }

        img_remainingTimeCover.fillAmount = 1f - buffInstance.GetRemainingTimeRatio();

    }
}
