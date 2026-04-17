using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPGDemo.Combat;
using RPGDemo.InteractionSystem;
using RPGDemo.Saving;
using RPGDemo.Weapons;
using RPGDemo.Inputs;
using RPGDemo.Attributes;
using UnityEngine;
using RPGDemo.Stats;
using UnityEngine.InputSystem;
using HSM;
using System.Linq;
using NewDialogueFrame;
using TMPro;

//[RequireComponent(typeof(ThirdPersonController))]
[RequireComponent(typeof(PlayerWeapon))]
[RequireComponent(typeof(PlayerAnimationHandler))]

public class Player : Character, ISaveable
{
    [SerializeField] InputReader input;
    [SerializeField] Transform characterModel;
    [SerializeField] DamageNumberWorldUI damageNumberUIPrefab;
    [SerializeField] Canvas damageNumberUIParent;
    [SerializeField] TextMeshProUGUI tmp_statePath;

    #region  set & get

    public InputReader Input => input;
    public PlayerLocomotion Locomotion => _locomotion;
    public Animator Animator => _animator;
    public Interactor Interactor => _interactor;
    public BaseStats BaseStats => _baseStats;
    public Health Health => _health;
    public Experience Experience => _experience;
    public PlayerWeapon Weapon => _weapon;
    public new PlayerAnimationHandler AnimationHandler => _animatorHandler;
    public PlayerCanversant PlayerCanversant => _playerCanversant;

    private PlayerLocomotion _locomotion;
    private Animator _animator;
    private BaseStats _baseStats;
    private Health _health;
    private Experience _experience;
    private Interactor _interactor;

    private PlayerWeapon _weapon;

    private PlayerCanversant _playerCanversant;
    #endregion

    //角色状态机
    HSM.StateMachine stateMachine;
    State rootState;
    [SerializeField] string statePath;

    private PlayerAnimationHandler _animatorHandler;

    public bool IsInCombat;
    public bool AttackPerformed;


    protected override void Awake()
    {
        base.Awake();
        _animatorHandler = GetComponent<PlayerAnimationHandler>();

        _locomotion = GetComponent<PlayerLocomotion>();
        _animator = GetComponent<Animator>();
        _interactor = GetComponentInChildren<Interactor>();


        _baseStats = GetComponent<BaseStats>();
        _health = GetComponent<Health>();
        _experience = GetComponent<Experience>();
        _weapon = GetComponent<PlayerWeapon>();

        _playerCanversant = GetComponent<PlayerCanversant>();

        //构建状态机
        rootState = new PlayerRoot(null, this);
        var builder = new StateMachineBuilder(rootState);
        stateMachine = builder.Build();
    }
    private void OnEnable()
    {
        _health.OnHealthChanged += OnHealthChanged;
        _health.OnDeath += OnDeath;

        Input.Attack += OnAttack;
    }



    private void OnDisable()
    {
        _health.OnDeath -= OnDeath;
        _health.OnHealthChanged -= OnHealthChanged;

        Input.Attack -= OnAttack;
    }

    void Start()
    {
        input.EnablePlayerActions();

    }

    void Update()
    {


        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            IsInCombat = !IsInCombat;

        }

        stateMachine.Tick(Time.deltaTime);
#if UNITY_EDITOR
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            _experience.GainExp(100);
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            _health.TakeDamage(10f);

        }
        statePath = SetPath(stateMachine.Root.Leaf());
        if (tmp_statePath != null)
            tmp_statePath.text = statePath;
#endif
    }

    private void OnAttack()
    {
        if (AnimationHandler.IsHandInteracting) return;
        if (!Locomotion.Grounded) return;

        AttackPerformed = true;
    }



    public static string SetPath(State s)
    {
        return string.Join(">", s.PathToRoot().AsEnumerable().Reverse().Select(n => n.GetType().Name));
    }

    public static Player GetInstance() => GameObject.FindGameObjectWithTag("Player")?.GetComponent<Player>();

    public void EnablePlayerControl() { input.EnablePlayerControl(); }
    public void DisablePlayerControl() { input.DisablePlayerAction(); }

    public void EnableLocomotion() => _locomotion.enabled = true;
    public void DisableLocomotion() => _locomotion.enabled = false;

    public void Move(Vector3 movement)
    {
        Locomotion.RootMotionMove(movement);
    }

    //根动画运动
    public void RootMotionMove(Vector3 movement, Quaternion deltaRotation)
    {
        Locomotion.RootMotionMove(movement, deltaRotation);
    }

    //传送
    public void TransTo(Transform point)
    {
        var rb = GetComponent<Rigidbody>();

        // 瞬移使用直接设置刚体位姿，避免MovePosition在同帧切换isKinematic时不生效。
        rb.position = point.position;
        rb.rotation = point.rotation;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnDeath()
    {
        DisablePlayerControl();
        AnimationHandler.PlayInterruptAnimation(PlayerAnimatorParamConfig.clipIDDeath, true);
        Fighter.DisableWeaponDamage();

    }

    private void OnHealthChanged(float value)
    {
        if (value > 0)
        {
            Animator.Play(Animator.StringToHash("Hit"));
            Animator.CrossFade(Animator.StringToHash("Hit"), 0.2f);
        }
    }




    enum PlayerSavingData
    {
        Position,
        Rotation,
        IsInCombat
    }
    JToken ISaveable.CapatureStateAsJToken()
    {
        JObject state = new JObject();
        IDictionary<string, JToken> stateDict = state;
        stateDict[PlayerSavingData.Position.ToString()] = transform.position.ToJToken();
        stateDict[PlayerSavingData.Rotation.ToString()] = transform.rotation.eulerAngles.ToJToken();
        stateDict[PlayerSavingData.IsInCombat.ToString()] = IsInCombat;

        return state;

    }

    void ISaveable.RestoreStateFromJToken(JToken s)
    {
        JObject state = s as JObject;
        IDictionary<string, JToken> stateDict = state;

        var rb = GetComponent<Rigidbody>();

        if (stateDict.ContainsKey(PlayerSavingData.Position.ToString()))
        {
            rb.position = stateDict[PlayerSavingData.Position.ToString()].ToVector3();

        }

        if (stateDict.ContainsKey(PlayerSavingData.Rotation.ToString()))
        {
            var eulerAngles = stateDict[PlayerSavingData.Rotation.ToString()].ToVector3();
            Quaternion rotation = Quaternion.Euler(eulerAngles);
            rb.rotation = rotation;
        }

        if (stateDict.ContainsKey(PlayerSavingData.IsInCombat.ToString()))
        {
            IsInCombat = stateDict[PlayerSavingData.IsInCombat.ToString()].ToObject<bool>();
        }


        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


}
