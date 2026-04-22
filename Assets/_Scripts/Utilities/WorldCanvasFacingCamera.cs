using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class WorldCanvasFacingCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] float minDistanceToCamera = 5f;
    [SerializeField] float maxDistanceToCamera = 15f;
    [SerializeField] bool ignoreYAxisFacing = true;

    Canvas canvas;
    CanvasGroup canvasGroup;

    Camera mainCamera;

    Transform currentTarget;
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        mainCamera = Camera.main;

        canvas.worldCamera = mainCamera;

        currentTarget = target == null ? transform : target;
    }

    private void LateUpdate()
    {
        if (ignoreYAxisFacing)
        {
            Vector3 dir = mainCamera.transform.forward;
            dir.y = 0;
            currentTarget.forward = dir.normalized;

        }
        else
        {
            currentTarget.LookAt(mainCamera.transform);
        }


        // 防止伤害数字贴相机太大，小于1米直接设为透明
        float distance = Vector3.Distance(currentTarget.position, Camera.main.transform.position);
        if (distance < minDistanceToCamera)
        {
            canvasGroup.alpha = 0.2f;
        }
        else if (distance > maxDistanceToCamera)
        {
            canvasGroup.alpha = 0f;
        }
        else
        {

            canvasGroup.alpha = 1f;
        }
    }


}
