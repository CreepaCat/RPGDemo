using System;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotion : MonoBehaviour
{
    //  [SerializeField] InputReader input;


    [Header("移动与旋转")]
    [SerializeField] float moveSpeed = 5.0f;
    [SerializeField] float sprintSpeed = 8.335f;
    [SerializeField] float rotationSpeed = 15f;
    [SerializeField] float rotationSmoothTime = 0.2f;
    [SerializeField] float speedChangeRate = 30f; //加速度



    [Header("地面检测")]
    [SerializeField] private bool _grounded = true;
    [SerializeField] float GroundedOffset = -0.14f; //用于粗糙地面检测
    public float GroundedRadius = 0.28f;
    public LayerMask GroundLayers;
    public bool RealGrounded => _grounded;
    public bool AssistedGrounded => _downStairing || _upStairing || IsCoyoteTime;
    public bool Grounded => RealGrounded || AssistedGrounded;


    [Header("跳跃与坠落")]
    [Space(10)]
    public float JumpHeight = 2f;
    public float Gravity = -15.0f;
    public bool IsCoyoteTime => _fallTimeoutDelta > 0f;

    [Space(10)]
    public float JumpTimeout = 0.50f; //跳跃冷却
    public float FallTimeout = 0.4f;//坠落延迟
    [SerializeField] private float jumpLockDuration = 0.2f; //起跳后短时间忽略接地退出

    [Header("楼梯参数")]
    [SerializeField] bool CheckStairs = false;
    [SerializeField] private float stairHeight = 0.45f;        // 最大台阶高度
    [SerializeField] private float stairSensorRadius = 0.5f;        // 胶囊体半径（你的角色胶囊半径）
    [SerializeField] private float stairSnapSpeed = 30f;       // 上台阶吸附速度（越大越快）
                                                               // [SerializeField] float checkDistance = 0.5f;
    [SerializeField] float stairUpMutiplier = 1f;
    [SerializeField] int upSatirSmoothStep = 3;
    [SerializeField] float stairDownMultiplier = 1f;
    [SerializeField] Transform upperSensor;
    [SerializeField] Transform lowerSensor;
    [SerializeField] Transform lowerSensor2;
    private bool _downStairing;
    private bool _upStairing;

    public float VerticalVelocity
    {
        get => _verticalVelocity;
        set => _verticalVelocity = value;
    }

    public bool CanMove = true;

    //CACHE
    Rigidbody rb;
    Vector3 movement;
    Transform mainCam;

    // float sprintVelocity = 1f;

    bool isSprinting = false;
    float _animationBlend;

    float _targetRotation;
    bool _hasDesiredRotation;
    float _speed;
    private float _rotationVelocity;
    Vector3 _cachedMoveDirection = Vector3.forward;

    //jump & fall
    float _verticalVelocity;
    float _fallTimeoutDelta;
    float _jumpTimeOutDelta;
    //float _jumpLockDelta;
    public bool IsJumping => _jumpTimeOutDelta > JumpTimeout * 0.5f;
    //public bool IsJumpLockActive => _jumpLockDelta > 0f;
    private float _terminalVelocity = 53.0f; //下坠最大速度

    public float JumpSpeed => Mathf.Sqrt(-2 * Gravity * JumpHeight);

    Vector3 rbVelocity;  //角色刚体速率，用于缓存Update里的计算结果，在fixedUpdate里赋值给rb
    //Vector3 stairTargetPos;

    Player player;
    InputReader input => player.Input;
    Animator _animator => player.Animator;

    public bool IsMoving => input.Direction.sqrMagnitude > 0.01f
    && rbVelocity.sqrMagnitude > 0.01f
    && CanMove
    && !player.AnimationHandler.IsInteracting;
    public bool RollPerformed;
    public bool JumpPerformed;



    const float ZeroF = 0f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // rb.isKinematic = true;
        rb.freezeRotation = true;
        rb.useGravity = false;

        player = GetComponent<Player>();

        mainCam = Camera.main.transform;
    }

    void Start()
    {
        input.EnablePlayerActions();
    }

    void OnEnable()
    {

        input.Sprint += OnSprint;
        input.Roll += OnRoll;
        input.Jump += OnJump;

    }
    void OnDisable()
    {

        input.Sprint -= OnSprint;
        input.Roll -= OnRoll;
        input.Jump -= OnJump;

    }

    private void OnRoll()
    {
        if (player.AnimationHandler.IsInteracting)
            return;
        if (!Grounded) return;
        RollPerformed = true;

    }

    private void OnSprint(bool performed)
    {
        isSprinting = performed;
    }

    private void OnJump()
    {
        if (player.AnimationHandler.IsInteracting || _jumpTimeOutDelta > 0f)
            return;
        if (!Grounded) return;
        JumpPerformed = true;
    }

    private void Update()
    {
        movement = new Vector3(input.Direction.x, 0f, input.Direction.y);
        UpdateDesiredFacing();
        GroundedCheck();
        if (CheckStairs)
        {
            StairsCheck();
        }
        CheckStartFalling();
        //  CheckJumping();
        player.Animator.SetFloat("VerticalSpeed", VerticalVelocity);
        _jumpTimeOutDelta -= Time.deltaTime; //跳跃冷却

    }

    private void FixedUpdate()
    {
        // 根动画控制期间跳过物理旋转，避免双驱动。
        if (player.AnimationHandler.IsInteracting && player.AnimationHandler.UsingRootMotion)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }
        rb.isKinematic = false;

        rbVelocity.y = _verticalVelocity;

        rb.linearVelocity = rbVelocity;

        ApplyFacingInFixedStep();
    }


    #region 移动与旋转

    public void CaculateMovement()
    {
        //交互型动画时不允许移动
        if (!CanMove || player.AnimationHandler.IsInteracting)
        {
            rbVelocity = new Vector3(0f, rbVelocity.y, 0f);
            _speed = 0f;
            _animationBlend = 0f;
            player.Animator.SetFloat(PlayerAnimatorParamConfig.animIDSpeed, _animationBlend);
            return;
        }

        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;

        if (input.Direction == Vector2.zero)
        {
            targetSpeed = 0.0f;

        }

        float currentHorizontalSpeed = new Vector3(rb.linearVelocity.x, 0.0f, rb.linearVelocity.z).magnitude;

        float speedOffset = 0.01f;
        float inputMagnitude = 1f; //todo：考虑手柄轻推输入

        // 加速或减速
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {

            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * speedChangeRate);

            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }
        Vector3 horizontalVelocity = CaculateDirection().normalized * _speed;
        var velocity = new Vector3(horizontalVelocity.x, _verticalVelocity, horizontalVelocity.z);
        rbVelocity = velocity;

        //移动动画混合
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        _animator.SetFloat(PlayerAnimatorParamConfig.animIDSpeed, _animationBlend);
        _animator.SetFloat(PlayerAnimatorParamConfig.animIDMotionSpeed, inputMagnitude);




    }


    // Update阶段只记录期望朝向，不直接改Transform。
    private void UpdateDesiredFacing()
    {
        _hasDesiredRotation = false;
        if (!CanMove) return;
        if (player.AnimationHandler.IsInteracting) return;
        if (input.Direction.sqrMagnitude <= 0.01f) return;

        Vector3 inputDirection = movement.normalized;
        _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                          mainCam.transform.eulerAngles.y;
        _hasDesiredRotation = true;
    }

    // 在Update中直接应用transform旋转，保持与输入采样同帧。
    private void ApplyFacingInFixedStep()
    {
        _cachedMoveDirection = transform.forward;

        if (!CanMove) return;
        if (!_hasDesiredRotation) return;
        if (player.AnimationHandler.IsInteracting) return;

        float rotation = Mathf.SmoothDampAngle(
            rb.rotation.eulerAngles.y,
            _targetRotation,
            ref _rotationVelocity,
            rotationSmoothTime,
            Mathf.Infinity,
            Time.fixedDeltaTime
        );

        Quaternion target = Quaternion.Euler(0f, rotation, 0f);
        rb.MoveRotation(target);
        _cachedMoveDirection = target * Vector3.forward;
    }
    //返回本帧统一方向，避免多处重复计算导致方向与旋转不同步
    public Vector3 CaculateDirection()
    {
        if (_cachedMoveDirection.sqrMagnitude < 0.0001f)
        {
            return transform.forward;
        }
        return _cachedMoveDirection;
    }

    //地面检测
    public void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        _grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);
        //  StartingFall();
        //设置动画bool
        player.Animator.SetBool(PlayerAnimatorParamConfig.animIDGrounded, Grounded);
    }

    private void CheckStartFalling()
    {


        if (_grounded || _upStairing || _downStairing)
        {
            _fallTimeoutDelta = FallTimeout; //重置掉落计时
            if (_verticalVelocity < 0.0f && !_upStairing)
            {
                _verticalVelocity = -2f; //在地面时数值速度设为-2,使之贴地
            }
        }
        else if (_fallTimeoutDelta >= 0.0f) //不马上掉落，更真实
        {
            _fallTimeoutDelta -= Time.deltaTime;

        }
    }

    //根动画移动
    internal void RootMotionMove(Vector3 movement)
    {
        // Debug.Log("根动画移动" + movement);
        var targetMovement = movement + new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f);
        rb.MovePosition(rb.position + targetMovement);
    }



    internal void RootMotionMove(Vector3 movement, Quaternion deltaRotation)
    {
        movement.y = 0f;

        var verticalDrop = Vector3.zero;


        if (RollPerformed) //翻滚时贴地
        {
            verticalDrop = new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f);
        }
        var targetMovement = movement + verticalDrop;
        rb.MovePosition(rb.position + targetMovement);

        if (deltaRotation != Quaternion.identity)
        {
            rb.MoveRotation(rb.rotation * deltaRotation);
            _cachedMoveDirection = (rb.rotation * deltaRotation) * Vector3.forward;
        }
    }


    #endregion

    #region 跳跃和掉落

    public void CheckJumping()
    {
        if (Grounded && _jumpTimeOutDelta < 0f)
        {
            if (player.AnimationHandler.IsInteracting) return; //动画交互时不允许跳跃
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _jumpTimeOutDelta = JumpTimeout;
                // _jumpLockDelta = jumpLockDuration;
                //自由落体公式
                float jumpSpeed = Mathf.Sqrt(-2 * Gravity * JumpHeight);
                // Vector3 targetDirection = CaculateDirection();
                _verticalVelocity = jumpSpeed;
                Vector3 horizontalVelocity = CaculateDirection().normalized * _speed * 0.5f;
                var velocity = new Vector3(horizontalVelocity.x, _verticalVelocity, horizontalVelocity.z);
                rbVelocity = velocity;
                Debug.Log("Jumping");
                player.AnimationHandler.PlayTargetAnimation(PlayerAnimatorParamConfig.clipIDJumping, true, false, 0.04f);
                player.Animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, false);


            }

        }
        _jumpTimeOutDelta -= Time.deltaTime; //跳跃冷却

    }

    public void SetJump()
    {
        _jumpTimeOutDelta = JumpTimeout;
        //自由落体公式
        float jumpSpeed = Mathf.Sqrt(-2 * Gravity * JumpHeight);
        _verticalVelocity = jumpSpeed;
        Vector3 horizontalVelocity = CaculateDirection().normalized * _speed * 0.5f;
        var velocity = new Vector3(horizontalVelocity.x, _verticalVelocity, horizontalVelocity.z);
        rbVelocity = velocity;
    }

    public void CaculateFalling()
    {
        if (_verticalVelocity >= 0f)
        {
            _verticalVelocity += Gravity * Time.deltaTime; //正常重力上升
        }
        if (_verticalVelocity < 0f && -_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * Time.deltaTime * 2; //两倍重力下落
        }

        if (_fallTimeoutDelta >= 0f) return;


        //手动计算应用坠落位移，覆盖根动画的位移计算,但会与Rolling根动画冲突
        Vector3 horizontalVelocity = CaculateDirection().normalized * _speed * 0.5f;
        var velocity = new Vector3(horizontalVelocity.x, _verticalVelocity, horizontalVelocity.z);
        rbVelocity = velocity;




    }
    internal void StopMovement()
    {
        rbVelocity = Vector3.zero;

    }




    #endregion
    #region 上下楼梯

    Collider[] lowerBlockes;
    Collider[] lowerBlockes2;
    Collider[] upperBlockes;

    public void StairsCheck()
    {
        _downStairing = false;
        _upStairing = false;
        if (!RealGrounded) return; //不在地面上不检测楼梯
        if (IsJumping) return; //跳跃时不检测楼梯
        if (input.Direction == Vector2.zero) return;        // 没在移动

        Vector3 currentPos = transform.position;
        Vector3 lowerOrigin = lowerSensor.position;
        Vector3 lowerOrigin2 = lowerSensor2.position;
        Vector3 upperOrigin = upperSensor.position;

        // 上楼梯检测

        lowerBlockes = new Collider[1];
        lowerBlockes2 = new Collider[1];
        upperBlockes = new Collider[1];

        bool lowerBlocked = Physics.OverlapSphereNonAlloc(lowerOrigin, stairSensorRadius,
                        lowerBlockes, GroundLayers, QueryTriggerInteraction.Ignore) > 0;
        bool lowerBlocked2 = Physics.OverlapSphereNonAlloc(lowerOrigin2, stairSensorRadius,
        lowerBlockes2, GroundLayers, QueryTriggerInteraction.Ignore) > 0;

        bool upperBlocked = Physics.OverlapSphereNonAlloc(upperOrigin, stairSensorRadius,
                    upperBlockes, GroundLayers, QueryTriggerInteraction.Ignore) > 0;

        //Debug.Log($"lowerBlocked{lowerBlocked}, lowerBlocked2 {lowerBlocked2}upperBlocked{upperBlocked}");

        Vector3 stairTargetPos = currentPos;
        if (lowerBlocked && lowerBlocked2 && !upperBlocked)
        {
            Debug.Log("检测到台阶");

            // 计算台阶顶面精确位置
            if (Physics.Raycast(upperOrigin, Vector3.down, out RaycastHit stepTopHit, 10f, GroundLayers))
            {
                stairTargetPos = new Vector3(currentPos.x, stepTopHit.point.y, currentPos.z);
            }
            if (transform.position.y > stairTargetPos.y + 0.02f) return; //说明不是上楼梯

            UpStairWalk(stairTargetPos);
            return;
        }

        if (_verticalVelocity > 0f) return; //在上升过程中不检测下楼梯，避免误判

        //todo:下楼梯检测
        //若三个检测器都没有被阻挡，下楼梯需要检测前后高度差
        if ((!lowerBlocked || !lowerBlocked2) && !upperBlocked)
        {
            // 计算台阶顶面精确位置
            if (Physics.Raycast(upperOrigin, Vector3.down, out RaycastHit stepTopHit, 10f, GroundLayers))
            {
                stairTargetPos = new Vector3(currentPos.x, stepTopHit.point.y, currentPos.z);
            }
            if (transform.position.y < stairTargetPos.y + 0.02f) return; //说明不是下楼梯
            float down = Mathf.Abs(stairTargetPos.y - transform.position.y);

            if (stairTargetPos != transform.position && down <= stairHeight && down > 0.02f)
            {
                Debug.Log("检测到下楼梯");
                _downStairing = true;
                var velocity = Mathf.Sqrt(-2 * Gravity * down) * -stairDownMultiplier;
                velocity = Mathf.Min(velocity, -2f);
                //下楼梯不能直接移动trans,容易卡住,只有检测到下坠时才lerp
                if (_verticalVelocity < velocity)
                {
                    transform.position = Vector3.Lerp(transform.position, stairTargetPos, Time.deltaTime * stairSnapSpeed);

                }
                //当前移动速度越大，需要的抓地速度就越大
                _verticalVelocity = velocity;

            }

        }

    }

    private void UpStairWalk(Vector3 stairTargetPos)
    {
        float rise = Mathf.Abs(stairTargetPos.y - transform.position.y);
        if (stairTargetPos != transform.position && rise <= stairHeight && rise > 0.02f)
        {
            _upStairing = true;
            var velocity = Mathf.Sqrt(-2 * Gravity * rise) * stairUpMutiplier;
            velocity = Mathf.Max(velocity, 2f);
            if (_verticalVelocity < velocity)
            {
                transform.position = Vector3.Lerp(transform.position, stairTargetPos,
               Time.deltaTime * stairSnapSpeed * rise * _speed / moveSpeed);

            }

            _verticalVelocity = velocity;

        }

    }

    #endregion
    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;
        Gizmos.DrawSphere(
            new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
            GroundedRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(
      lowerSensor.position,
      stairSensorRadius);
        Gizmos.DrawSphere(
      lowerSensor2.position,
      stairSensorRadius);
        Gizmos.DrawSphere(
    upperSensor.position,
    stairSensorRadius);
    }


}
