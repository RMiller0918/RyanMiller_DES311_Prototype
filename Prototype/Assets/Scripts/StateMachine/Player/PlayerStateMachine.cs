using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStateMachine : MonoBehaviour
{
    [Header("Components")] 
    [SerializeField] private PlayerControls _playerControls;
    [field: SerializeField] public Transform MainCamera { get; private set; }
    [field: SerializeField] public PlayerBaseState CurrentState { get; set; }
    [SerializeField] private PlayerStateFactory _factory;
    [field: SerializeField] public CharacterController CharCont { get; private set; }

    [field: Header("Player Stats")]
    [field: SerializeField] [field: Range(100,500)] public int Health { get; set; }
    [field: SerializeField] [field: Range(100,200)] public int Mana { get; set; }

    [field: Header("Animation")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public int RangeSetUpHash { get; private set; }
    [field: SerializeField] public int RangeFireTriggerHash { get; private set; }

    [field: SerializeField] public Vector2 MoveInput { get; private set; }

    [field: Header("State Booleans")]
    [field: SerializeField] public bool Moving { get; private set; }
    [field: SerializeField] public bool Sprinting { get; private set; }
    [field: SerializeField] public bool Crouching { get; private set; }
    [field: SerializeField] public bool Grounded { get; private set; }
    [field: SerializeField] public bool Jumping { get; private set; }
    [field: SerializeField] public bool Falling { get; private set; }
    [field: SerializeField] public bool Attacking { get; private set; }
    [field: SerializeField] public bool Aiming { get; private set; }
    [field: SerializeField] public bool TeleportSetUp { get; private set; }
    [field: SerializeField] public bool PullEnemySetUp { get; private set; }
    [field: SerializeField] public bool Healing { get; private set; }
    [field: SerializeField] public bool Interacting { get; private set; }

    [field: Header("Repeat action blockers")]
    [field: SerializeField] public bool NewJumpRequired { get; set; }
    [field: SerializeField] public bool NewAttackRequired { get; set; }
    [field: SerializeField] public bool RangedAttackSetUp { get; set; }
    public Coroutine RangedAttackCooldown { get; set; }

    [field: Header("Gravity")]
    [field: SerializeField] public float Gravity { get; set; }
    [field: SerializeField] public float GroundedGravity { get; } = .5f;

    [field: Header("Jumping")]
    [field: SerializeField] public float InitialJumpVelocity { get; set; }
    [field: SerializeField][field: Range(2,4)] public float _maxJumpHeight { get; private set; }
    [field: SerializeField][field: Range(0.5f,1)] public float _maxJumpTime { get; private set; }
    [field: SerializeField] public bool IsJumping { get; set; }

    [field: Header("Movement Vector")]
    public Vector3 MoveVelocity;
    [field: SerializeField] public float MoveVelocityX { get => MoveVelocity.x; set => MoveVelocity.x = value; }
    [field: SerializeField] public float MoveVelocityY { get => MoveVelocity.y; set => MoveVelocity.y = value; }
    [field: SerializeField] public float MoveVelocityZ { get => MoveVelocity.z; set => MoveVelocity.z = value; }

    [field: SerializeField] public Vector3 AppliedMoveVelocity;
    [field: SerializeField] public float AppliedMoveVelocityY { get => AppliedMoveVelocity.y; set => AppliedMoveVelocity.y = value; }

    [field: SerializeField] public RangedMainOrbScript OrbScript { get; private set; }
    [field: SerializeField] public GameObject BoltPrefab { get; private set; }
    [field: SerializeField] public Transform BoltTransform { get; private set; }

    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        RangeSetUpHash = Animator.StringToHash("RangeSetup");
        RangeFireTriggerHash = Animator.StringToHash("FireRangeBolt");
        CharCont = GetComponent<CharacterController>();
        MainCamera = Camera.main.transform;
        _playerControls = new PlayerControls();

        _playerControls.MainControls.Move.performed += OnMovementInput;
        _playerControls.MainControls.Move.started += OnMovementInput;
        _playerControls.MainControls.Move.canceled += OnMovementInput;
        _playerControls.MainControls.Jump.performed += ctx => Jumping = ctx.ReadValueAsButton();
        _playerControls.MainControls.Jump.performed += ctx => NewJumpRequired = false;
        _playerControls.MainControls.Sprint.performed += ctx => Sprinting = ctx.ReadValueAsButton();
        _playerControls.MainControls.Crouch.performed += ctx => Crouching = ctx.ReadValueAsButton();
        _playerControls.MainControls.Teleport.performed += ctx => TeleportSetUp = ctx.ReadValueAsButton();
        _playerControls.MainControls.Attack.performed += ctx => Attacking = ctx.ReadValueAsButton();
        _playerControls.MainControls.Aiming.performed += ctx => Aiming = ctx.ReadValueAsButton();
        _playerControls.MainControls.Interact.performed += ctx => Interacting = ctx.ReadValueAsButton();

        SetUpJumpVariables();

        _factory = new PlayerStateFactory(this);
        CurrentState = _factory.Grounded();
        CurrentState.EnterState();
        OrbScript = FindObjectOfType<RangedMainOrbScript>();
    }

    private void SetupAnimationVariables()
    {
        RangeSetUpHash = Animator.StringToHash("RangeSetup");
        RangeFireTriggerHash = Animator.StringToHash("FireRangeBolt");
    }

    private void SetUpJumpVariables()
    {
        var timeToApex = _maxJumpTime / 2;
        Gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        InitialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    #region Callbacks Functions
    private void OnMovementInput(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
        Moving = MoveInput.x != 0 || MoveInput.y != 0;
    }
    #endregion
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        CurrentState.UpdateStates();
        AppliedMoveVelocity.x = MoveVelocity.x;
        AppliedMoveVelocity.z = MoveVelocity.z;
        CharCont.Move(AppliedMoveVelocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        Grounded = CharCont.isGrounded;
        //Grounded = Physics.Raycast(transform.position, Vector3.down, 0.1f);
        /*
        if (!CharCont.isGrounded)
            AppliedMoveVelocityY += Gravity * Time.deltaTime;
        else
            AppliedMoveVelocityY = GroundedGravity;
        */
    }


#region animation events

    public void TriggerStart()
    {
        OrbScript.TriggerStart();
    }

    public void FireBoltTrigger()
    {

    }

#endregion

    private void OnEnable()
    {
        _playerControls.MainControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.MainControls.Disable();
    }

    private void OnDrawGizmos()
    {
        if (Grounded)
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.1f);
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.1f);
            Gizmos.color = Color.red;
        }
    }
}
