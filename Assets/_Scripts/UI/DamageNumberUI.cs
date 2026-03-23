using System;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DamageNumberUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI tmp_damageNumber;

    CanvasGroup canvasGroup;
    Camera cam;

    RectTransform canvasRect;
    RectTransform damageNumberRect;


    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        cam = Camera.main;
        canvasGroup.alpha = 1f;

        damageNumberRect = GetComponent<RectTransform>();
    }
    void Start()
    {
        //
        damageNumberRect.DOMoveY(damageNumberRect.position.y + 100f, 2f);
        canvasGroup.DOFade(0, 2f);


        Destroy(gameObject, 3f);
    }

    public void SetTextNumber(float v)
    {
        tmp_damageNumber.text = v.ToString("f1");
    }

    public void SetInitPosition(Vector3 initPos, Canvas parentCanvas)
    {
        Vector3 woeldPos = initPos;
        Vector3 screenPos = cam.WorldToScreenPoint(woeldPos);
        Camera uiCame = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : cam;

        canvasRect = parentCanvas.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, uiCame,
                out Vector2 localPos))
        {
            damageNumberRect.anchoredPosition = localPos;
        }

    }
}
