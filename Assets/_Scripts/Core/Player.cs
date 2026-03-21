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

[RequireComponent(typeof(ThirdPersonController))]
[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(PlayerAnimatorHandler))]

public class Player : Character, ISaveable
{
    public ThirdPersonController TpController => _tpController;

    public Animator Animator => _animator;
    public CharacterController CharacterController => _characterController;
    public PlayerInputs Input => _input;
    public Interactor Interactor => _interactor;
    public BaseStats BaseStats => _baseStats;
    public Health Health => _health;
    public Experience Experience => _experience;
    public Fighter Fighter => _fighter;
    public Weapon Weapon => _weapon;
    public PlayerAnimatorHandler AnimatorHandler => _animatorHandler;

    private ThirdPersonController _tpController;
    private CharacterController _characterController;
    private Animator _animator;
    private PlayerInputs _input;
    private BaseStats _baseStats;
    private Health _health;
    private Experience _experience;
    private Interactor _interactor;

    private Fighter _fighter;
    private Weapon _weapon;

    //[SerializeField] private InputReader inputReader;



    private PlayerAnimatorHandler _animatorHandler;


    private void Awake()
    {
        _tpController = GetComponent<ThirdPersonController>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _input = GetComponent<PlayerInputs>();

        _interactor = GetComponentInChildren<Interactor>();


        _baseStats = GetComponent<BaseStats>();
        _health = GetComponent<Health>();
        _experience = GetComponent<Experience>();
        _fighter = GetComponent<Fighter>();
        _weapon = GetComponent<Weapon>();

        _animatorHandler = GetComponent<PlayerAnimatorHandler>();
    }

    void Start()
    {
        _baseStats.SetLevelByTotalExp(100);

    }

    void Update()
    {
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            _experience.GainExp(100);
        }

        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            _health.TakeDamage(10);
        }
    }

    public static Player GetInstance() => GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

    public void EnableInput() => _input.EnablePlayerInput();
    public void DisableInput() => _input.DisablePlayerInput();

    public void EnableCharacterController() => _characterController.enabled = true;
    public void DisableCharacterController() => _characterController.enabled = false;

    public void Move(Vector3 movement)
    {
        CharacterController.Move(movement);
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
