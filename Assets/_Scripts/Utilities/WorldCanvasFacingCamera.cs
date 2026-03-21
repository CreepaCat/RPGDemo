using UnityEngine;

public class WorldCanvasFacingCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    Camera mainCamera;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            transform.forward = mainCamera.transform.forward;

        }
        else
        {
            target.forward = mainCamera.transform.forward;
        }

    }
}
