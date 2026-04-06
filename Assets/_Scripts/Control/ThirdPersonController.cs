using RPGDemo;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace RPGDemo.Inputs
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        public bool IsMoving => _input.move.sqrMagnitude > 0.01f || _speed > 0.01;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f; //下坠最大速度


        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _rollTimeDelta;
        private float _attackTimeoutDelta;

        //custom
        public float _attackTimeout = 0.2f;
        private bool isAttacking => _animator.GetBool(PlayerAnimatorParamConfig.animIDAttack);

        [SerializeField] private float _rollTimeOut = 0.1f;

        [field: SerializeField] public bool CanDoCombo { get; private set; }




#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private PlayerInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private Player _player;


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

        }


        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _player = GetComponent<Player>();

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _rollTimeDelta = _rollTimeOut;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);


            if (!_player.AnimationHandler.IsInteracting)
            {
                //Move();
                HandleRollInput();
                HandleAttackInput();
                HandleInteractInput();
            }



            // JumpAndGravity();
            GroundedCheck();

            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.Locked;
                //_player.EnableInput();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                // _player.DisableInput();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void HandleInteractInput()
        {
            if (_input.interact)
            {
                _input.interact = false;
                _player.Interactor.DoInteract();

            }
        }


        private void HandleRollInput()
        {
            _rollTimeDelta += Time.deltaTime;
            if (_rollTimeDelta < _rollTimeOut) return;

            if (_input.roll)
            {
                _rollTimeDelta = 0f;

                //旋转到对应方向，翻滚时若无方向输入，则以角色的朝向作为翻滚方向
                Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
                if (inputDirection.magnitude > _threshold)
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      _mainCamera.transform.eulerAngles.y;
                }
                else
                {
                    _targetRotation = transform.eulerAngles.y;
                }
                transform.rotation = Quaternion.Euler(0.0f, _targetRotation, 0.0f);

                _player.AnimationHandler.PlayTargetAnimation(PlayerAnimatorParamConfig.clipIDRolling, true, true, 0.1f);
                _input.roll = false;
            }
        }


        /// <summary>
        /// 自定义函数
        /// </summary>
        private void HandleAttackInput()
        {
            _attackTimeoutDelta += Time.deltaTime;
            if (!Grounded) return;
            if (_attackTimeoutDelta < _attackTimeout) return;

            // if (_input.heaveyAttack)
            // {
            //     _player.Weapon.HeaveAttack();
            //     _input.heaveyAttack = false; //防止长按连续重击
            //
            // }
            if (_input.lightAttack)
            {
                if (CanDoCombo)
                {
                    _player.AnimationHandler.UpdateCanDoCombo(true);
                    _player.Weapon.HandleWeaponCombo();
                    _player.AnimationHandler.UpdateCanDoCombo(false);
                }
                else
                {
                    // _player.Weapon.LightAttack();
                }

            }

        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(PlayerAnimatorParamConfig.animIDGrounded, Grounded);
            }
        }

        /// <summary>
        ///手搓旋转相机，写法值得借鉴
        /// </summary>
        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }


        // Vector3 inputDirection =>new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
        public void Move()
        {
            Debug.Log("Palyer is Moving");
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 targetDirection = CaculateDirection();
            // move the player
            //  if (_controller.enabled)
            //{
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            // }


            // update animator if using character
            // if (_hasAnimator)
            // {
            //     if (!_input.enabled) return;
            //  Debug.Log("anima set float speed" + _animationBlend);
            _animator.SetFloat(PlayerAnimatorParamConfig.animIDSpeed, _animationBlend);
            _animator.SetFloat(PlayerAnimatorParamConfig.animIDMotionSpeed, inputMagnitude);
            // }
        }

        public void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // // update animator if using character
                // if (_hasAnimator)
                // {
                //     //_animator.SetBool(PlayerAnimatorParaConfig.animIDJump, false);
                //     _animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, false);
                // }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }


                //项目暂不使用跳跃
                // Jump
                // if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                // {
                //     // the square root of H * -2 * G = how much velocity needed to reach desired height
                //     _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                //
                //     // update animator if using character
                //     if (_hasAnimator)
                //     {
                //         _animator.SetBool(_animIDJump, true);
                //     }
                // }
                //
                // // jump timeout
                // if (_jumpTimeoutDelta >= 0.0f)
                // {
                //     _jumpTimeoutDelta -= Time.deltaTime;
                // }
            }
            else
            {
                // reset the jump timeout timer
                // _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    // if (_hasAnimator)
                    // {
                    //     _animator.SetBool(PlayerAnimatorParamConfig.animIDIsFalling, true);
                    //     _player.AnimatorHandler.PlayTargetAnimation(PlayerAnimatorParamConfig.clipIDFreeFall, true, 0.04f);

                    // }
                    //手动计算应用坠落位移，覆盖根动画的位移计算,但会与Rolling根动画冲突
                    Vector3 targetDirection = CaculateDirection();

                    // move the player
                    if (_controller.enabled)
                    {
                        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) * 0.5f +
                                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                    }
                }

                // if we are not grounded, do not jump
                // _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private Vector3 CaculateDirection()
        {
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
            {
                //if(_input.enabled == false) return;
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            return Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        #region 动画事件处理
        public void OnFootstep()
        {

            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }

        }

        public void OnLand()
        {

            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);

        }

        public void OnAttackOver()
        {
            // _player.TpController.OnAttackOver();

            Debug.Log("攻击结束");
            // _input.lightAttack = false;
            //_input.chargeAttack = false;
            _attackTimeoutDelta = 0f;
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }


    }
}
