using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DamageNumberWorldUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp_damageNumber;
    [SerializeField] float fadeDurantion = 2f;
    [SerializeField] float moveDuration = 2f;
    [SerializeField] float yOffset = 2f;
    [SerializeField] float textEndScale = 1.5f;

    CanvasGroup canvasGroup;
    Camera cam;
    Vector3 offsetToCam;

    //todo:与相机保持相对位置


    private void Awake()
    {
        cam = Camera.main;
        tmp_damageNumber ??= GetComponentInChildren<TextMeshProUGUI>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        transform.DOMoveY(transform.position.y + yOffset, moveDuration);
        canvasGroup.DOFade(0, fadeDurantion);
        tmp_damageNumber.transform.DOScale(new Vector3(textEndScale, textEndScale, 1), moveDuration);
        Destroy(gameObject, fadeDurantion + 1f);
    }

    private void LateUpdate()
    {
        // 防止伤害数字贴相机太大，小于1米直接设为透明
        float distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        if (distance < 1f)
        {
            canvasGroup.alpha = 0f;
        }

    }



    public void SetTextNumber(float v)
    {
        tmp_damageNumber.text = v.ToString("f1");
    }


}
