using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteAlways]
public class PlayerCameraRotator : MonoBehaviour
{
    [Header("相机")]
    [Tooltip("相机follow目标")]
    [Required] public GameObject CinemachineCameraTarget;
    [Required] public InputReader input;
    [SerializeField] float offsetY = 1.37f;


    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    Vector2 look;
    bool isDeviceMouse;
    Player player;



    private void Awake()
    {
        player = Player.GetInstance();
    }

    private void OnEnable()
    {
        input.Look += OnLook;
    }
    private void OnDisable()
    {
        input.Look -= OnLook;
    }

    private void OnLook(Vector2 look, bool isDeviceMouse)
    {
        this.look = look;
        this.isDeviceMouse = isDeviceMouse;

    }

    private void Update()
    {
        var followPos = player.transform.position;
        followPos.y += offsetY;
        transform.position = followPos;

    }
    private void LateUpdate()
    {
        CameraRotation();
    }


    private void CameraRotation()
    {
        if (!LockCameraPosition && look.sqrMagnitude > 0.01f)
        {
            float deltaTimeMultiplier = isDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += look.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // 虚拟相机跟随目标
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
            _cinemachineTargetYaw, 0.0f);
    }
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}
