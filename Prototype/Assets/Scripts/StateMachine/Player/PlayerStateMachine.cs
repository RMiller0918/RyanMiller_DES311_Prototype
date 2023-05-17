using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStateMachine : MonoBehaviour, ILightable, IDamageable
{
    [Header("Components")] 
    [SerializeField] private PlayerControls _playerControls;
    [field: SerializeField] public Transform MainCamera { get; private set; }
    [field: SerializeField] public PlayerBaseState CurrentState { get; set; }
    [SerializeField] private PlayerStateFactory _factory;
    [field: SerializeField] public CharacterController CharCont { get; private set; }

    [field: Header("Player Stats")]
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] [field: Range(100,500)] public float Health { get; set; }
    [field: SerializeField] public int MaxMana { get; private set; }
    [field: SerializeField] [field: Range(100,200)] public float Mana { get; set; }

    [field: Header("Animation")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public int RangeSetUpHash { get; private set; }
    [field: SerializeField] public int RangeFireTriggerHash { get; private set; }
    [field: SerializeField] public int MeleeHash { get; private set; }
    [field: SerializeField] public int TeleportSetupHash { get; private set; }
    [field: SerializeField] public int TeleportTriggerHash { get; private set; }
    [field: SerializeField] public int HealTriggerHash { get; private set; }
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
    [field: SerializeField] public bool NewTeleSetUpRequired { get; set; }
    [field: SerializeField] public bool NewPullRequired { get; set; }
    [field: SerializeField] public bool NewHealRequired { get; set; }
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

    [field: Header("Ranged")]
    [field: SerializeField] public RangedMainOrbScript OrbScript { get; private set; }
    [field: SerializeField] public GameObject BoltPrefab { get; private set; }
    [field: SerializeField] public Transform BoltTransform { get; private set; }
    [field: SerializeField] public bool BoltFired { get; set; }

    [Header("Attacks")] 
    [SerializeField] public int MeleeCountHash; 
    [field: SerializeField] public int MeleeCount { get; set; }
    [field: SerializeField] public GameObject[] MeleeColliders { get; private set; }

    [field: Header("Teleport")]
    [field: SerializeField] public bool IsTeleporting { get; set; }

    [field:Header("Lighting")]
    [field: SerializeField] public bool Lit { get; private set; }

    [field:Header("UI")]
    [field: SerializeField] public TeleportMarker TeleMarker { get; private set; }
    [field: SerializeField] public HealthBar HBar { get; private set; }
    [field: SerializeField] public HealthBar MBar { get; private set; } //using same script for Mana as Health bar.
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        RangeSetUpHash = Animator.StringToHash("RangeSetup");
        RangeFireTriggerHash = Animator.StringToHash("FireRangeBolt");
        MeleeHash = Animator.StringToHash("Melee");
        MeleeCountHash = Animator.StringToHash("MeleeCount");
        TeleportSetupHash = Animator.StringToHash("TeleportSetup");
        TeleportTriggerHash = Animator.StringToHash("Teleport");
        HealTriggerHash = Animator.StringToHash("Heal");
        CharCont = GetComponent < CharacterController>();
        MainCamera = Camera.main.transform;
        _playerControls = new PlayerControls();

        _playerControls.MainControls.Move.performed += OnMovementInput;
        _playerControls.MainControls.Move.started += OnMovementInput;
        _playerControls.MainControls.Move.canceled += OnMovementInput;
        _playerControls.MainControls.Jump.performed += ctx =>
        {
            Jumping = ctx.ReadValueAsButton();
            NewJumpRequired = false;
        };
        _playerControls.MainControls.Sprint.performed += ctx => Sprinting = ctx.ReadValueAsButton();
        _playerControls.MainControls.Crouch.performed += ctx => Crouching = ctx.ReadValueAsButton();
        _playerControls.MainControls.Teleport.performed += ctx =>
        {
            TeleportSetUp = ctx.ReadValueAsButton();
            NewTeleSetUpRequired = false;
        };
        _playerControls.MainControls.Attack.performed += ctx =>
        {
            Attacking = ctx.ReadValueAsButton();
            NewAttackRequired = false;
        };
        _playerControls.MainControls.Aiming.performed += ctx => Aiming = ctx.ReadValueAsButton();
        _playerControls.MainControls.Interact.performed += ctx => Interacting = ctx.ReadValueAsButton();
        _playerControls.MainControls.PullEnemy.performed += ctx =>
        {
            PullEnemySetUp = ctx.ReadValueAsButton();
            NewPullRequired = false;
        };
        _playerControls.MainControls.Healing.performed += ctx =>
        {
            Healing = ctx.ReadValueAsButton();
            NewHealRequired = false;
        };

        SetUpJumpVariables();

        _factory = new PlayerStateFactory(this);
        CurrentState = _factory.Grounded();
        CurrentState.EnterState();
        OrbScript = FindObjectOfType<RangedMainOrbScript>();
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
        MaxHealth = (int)Health;
        MaxMana = (int)Mana;
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
        ManaRegen();
    }

    private void CheckGrounded()
    {
        Grounded = CharCont.isGrounded;
    }

    private void ManaRegen()
    {
        if (Lit || !(Mana < MaxMana)) return;
        Mana += 2f * Time.deltaTime;
        Mana = Mathf.Clamp(Mana, 0, MaxMana);
        MBar.UpdateHealthBar(MaxMana, Mana);
    }


    #region animation events

    public void TriggerStart()
    {
        OrbScript.TriggerStart(true);
    }

    public void FireBolt()
    {
        BoltFired = true;
        Mana -= 15f;
        Mana = Mathf.Clamp(Mana, 0, MaxMana);
        var rotation = new Vector3(MainCamera.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        Instantiate(BoltPrefab, BoltTransform.position, Quaternion.Euler(rotation));
        BoltFired = false;
    }

    public void EnableRightMeleeCollider()
    {
        MeleeColliders[0].SetActive(true);
    }
    public void EnableLeftMeleeCollider()
    {
        MeleeColliders[1].SetActive(true);
    }

    public void DisableMeleeColliders()
    {
        foreach (var obj in MeleeColliders)
        {
            obj.SetActive(false);
        }
    }
    #endregion

    #region Interfaces

    public void HandleHitByLight(int lightValue)
    {
        Lit = lightValue > 25;
    }

    public void HandleDamage(int damageValue)
    {
        var newHealth = Health - damageValue;
        Health = Mathf.Lerp(Health, newHealth, 5f * Time.deltaTime);
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        HBar.UpdateHealthBar(MaxHealth, Health);
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
