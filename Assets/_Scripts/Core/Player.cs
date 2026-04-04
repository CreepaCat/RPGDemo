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
[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(PlayerAnimationHandler))]

public class Player : Character, ISaveable
{
    [SerializeField] InputReader input;
    [SerializeField] Transform characterModel;
    [SerializeField] DamageNumberWorldUI damageNumberUIPrefab;
    [SerializeField] Canvas damageNumberUIParent;
    [SerializeField] TextMeshProUGUI tmp_statePath;
    public bool IsCombating => _fighter.HasTarget();

    #region  set & get

    public InputReader Input => input;
    public PlayerLocomotion Locomotion => _locomotion;
    public Animator Animator => _animator;
    public Interactor Interactor => _interactor;
    public BaseStats BaseStats => _baseStats;
    public Health Health => _health;
    public Experience Experience => _experience;
    public Fighter Fighter => _fighter;
    public Weapon Weapon => _weapon;
    public PlayerAnimationHandler AnimatorHandler => _animatorHandler;
    public PlayerCanversant PlayerCanversant => _playerCanversant;

    private PlayerLocomotion _locomotion;
    //private CharacterController _characterController;
    private Animator _animator;
    // private PlayerInputs _input;
    private BaseStats _baseStats;
    private Health _health;
    private Experience _experience;
    private Interactor _interactor;

    private Fighter _fighter;
    private Weapon _weapon;

    private PlayerCanversant _playerCanversant;
    #endregion

    //[SerializeField] private InputReader inputReader;

    //角色状态机
    HSM.StateMachine stateMachine;
    State rootState;
    [SerializeField] string statePath;



    private PlayerAnimationHandler _animatorHandler;


    private void Awake()
    {
        // _tpController = GetComponent<ThirdPersonController>();
        _locomotion = GetComponent<PlayerLocomotion>();
        //  _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();


        _interactor = GetComponentInChildren<Interactor>();


        _baseStats = GetComponent<BaseStats>();
        _health = GetComponent<Health>();
        _experience = GetComponent<Experience>();
        _fighter = GetComponent<Fighter>();
        _weapon = GetComponent<Weapon>();

        _animatorHandler = GetComponent<PlayerAnimationHandler>();
        _playerCanversant = GetComponent<PlayerCanversant>();

        //构建状态机
        rootState = new PlayerRoot(null, this);
        var builder = new StateMachineBuilder(rootState);
        stateMachine = builder.Build();
    }

    void Start()
    {
        _baseStats.SetLevelByTotalExp(100);

        input.EnablePlayerActions();

    }

    void Update()
    {


        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            _experience.GainExp(100);
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            _health.TakeDamage(10f);
            // DamageNumberWorldUI damageNumberUI = Instantiate(damageNumberUIPrefab);
            // //
            // damageNumberUI.transform.position = characterModel.position + Vector3.up * 2f;
            // damageNumberUI.SetTextNumber(10f);
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SetCursorState();
        }

        stateMachine.Tick(Time.deltaTime);
        statePath = SetPath(stateMachine.Root.Leaf());
        tmp_statePath.text = statePath;
    }

    private void SetCursorState()
    {
        Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public static string SetPath(State s)
    {
        return string.Join(">", s.PathToRoot().AsEnumerable().Reverse().Select(n => n.GetType().Name));
    }

    public static Player GetInstance() => GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    public void EnablePlayerControl() { input.EnablePlayerControl(); }
    public void DisablePlayerControl() { input.DisablePlayerAction(); }

    public void EnableLocomotion() => _locomotion.enabled = true;
    public void DisableLocomotion() => _locomotion.enabled = false;

    public void Move(Vector3 movement)
    {
        Locomotion.Move(movement);
    }

    public void Move(Vector3 movement, Quaternion deltaRotation)
    {
        Locomotion.Move(movement, deltaRotation);
    }




    enum PlayerSavingData
    {
        Position,
        Rotation,
    }
    JToken ISaveable.CapatureStateAsJToken()
    {
        JObject state = new JObject();
        IDictionary<string, JToken> stateDict = state;
        stateDict[PlayerSavingData.Position.ToString()] = transform.position.ToJToken();
        stateDict[PlayerSavingData.Rotation.ToString()] = transform.rotation.eulerAngles.ToJToken();

        return state;

    }

    void ISaveable.RestoreStateFromJToken(JToken s)
    {
        JObject state = s as JObject;
        IDictionary<string, JToken> stateDict = state;

        if (stateDict.ContainsKey(PlayerSavingData.Position.ToString()))
        {
            transform.position = stateDict[PlayerSavingData.Position.ToString()].ToVector3();

        }

        if (stateDict.ContainsKey(PlayerSavingData.Rotation.ToString()))
        {
            transform.eulerAngles = stateDict[PlayerSavingData.Rotation.ToString()].ToVector3();
        }
    }
}
