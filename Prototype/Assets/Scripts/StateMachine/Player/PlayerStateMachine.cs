using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStateMachine : MonoBehaviour, ILightable, IDamageable
{
    #region Components
    [Header("Components")] 
    [SerializeField] private PlayerControls _playerControls;
    [field: SerializeField] public Transform MainCamera { get; private set; }
    [field: SerializeField] public PlayerBaseState CurrentState { get; set; }
    [SerializeField] private PlayerStateFactory _factory;
    [field: SerializeField] public CharacterController CharCont { get; private set; }
    #endregion
    #region Player Stats
    [field: Header("Player Stats")]
    [field: SerializeField] public int MaxHealth { get; private set; }
    [field: SerializeField] [field: Range(100,500)] public float Health { get; set; }
    [field: SerializeField] public int MaxMana { get; private set; }
    [field: SerializeField] [field: Range(100,200)] public float Mana { get; set; }
    #endregion
    #region Animations
    [field: Header("Animation")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public int RangeSetUpHash { get; private set; }
    [field: SerializeField] public int RangeFireTriggerHash { get; private set; }
    [field: SerializeField] public int MeleeHash { get; private set; }
    [field: SerializeField] public int TeleportSetupHash { get; private set; }
    [field: SerializeField] public int TeleportTriggerHash { get; private set; }
    [field: SerializeField] public int HealTriggerHash { get; private set; }
    [field: SerializeField] public Vector2 MoveInput { get; private set; }
    #endregion
    #region State Booleans
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
    #endregion
    #region Repeat Action Blockers
    [field: Header("Repeat action blockers")]
    [field: SerializeField] public bool NewJumpRequired { get; set; }
    [field: SerializeField] public bool NewAttackRequired { get; set; }
    [field: SerializeField] public bool NewTeleSetUpRequired { get; set; }
    [field: SerializeField] public bool NewPullRequired { get; set; }
    [field: SerializeField] public bool NewHealRequired { get; set; }
    [field: SerializeField] public bool RangedAttackSetUp { get; set; }
    public Coroutine RangedAttackCooldown { get; set; }
    #endregion
    #region Gravity
    [field: Header("Gravity")]
    [field: SerializeField] public float Gravity { get; set; }
    [field: SerializeField] public float GroundedGravity { get; } = .5f;
    #endregion
    #region Jumping
    [field: Header("Jumping")]
    [field: SerializeField] public float InitialJumpVelocity { get; set; }
    [field: SerializeField][field: Range(2,4)] public float _maxJumpHeight { get; private set; }
    [field: SerializeField][field: Range(0.5f,1)] public float _maxJumpTime { get; private set; }
    [field: SerializeField] public bool IsJumping { get; set; }
    #endregion
    #region Movement Vectors
    [field: Header("Movement Vector")]
    public Vector3 MoveVelocity;
    [field: SerializeField] public float MoveVelocityX { get => MoveVelocity.x; set => MoveVelocity.x = value; }
    [field: SerializeField] public float MoveVelocityY { get => MoveVelocity.y; set => MoveVelocity.y = value; }
    [field: SerializeField] public float MoveVelocityZ { get => MoveVelocity.z; set => MoveVelocity.z = value; }

    [field: SerializeField] public Vector3 AppliedMoveVelocity;
    [field: SerializeField] public float AppliedMoveVelocityY { get => AppliedMoveVelocity.y; set => AppliedMoveVelocity.y = value; }
    #endregion
    #region Ranged
    [field: Header("Ranged")]
    [field: SerializeField] public RangedMainOrbScript OrbScript { get; private set; }
    [field: SerializeField] public GameObject BoltPrefab { get; private set; }
    [field: SerializeField] public Transform BoltTransform { get; private set; }
    [field: SerializeField] public bool BoltFired { get; set; }
    #endregion
    #region Attacks
    [Header("Attacks")] 
    [SerializeField] public int MeleeCountHash; 
    [field: SerializeField] public int MeleeCount { get; set; }
    [field: SerializeField] public GameObject[] MeleeColliders { get; private set; }
    #endregion
    #region Teleport
    [field: Header("Teleport")]
    [field: SerializeField] public bool IsTeleporting { get; set; }
    #endregion
    #region Lighting
    [field:Header("Lighting")]
    [field: SerializeField] public bool Lit { get; private set; }
    #endregion
    #region UI
    [field:Header("UI")]
    [field: SerializeField] public TeleportMarker TeleMarker { get; private set; }
    [field: SerializeField] public HealthBar HBar { get; private set; }
    [field: SerializeField] public HealthBar MBar { get; private set; } //using same script for Mana as Health bar.
    [field: SerializeField] public IconScript RangeIcon { get; private set; } //using same script for Mana as Health bar.
    [field: SerializeField] public IconScript TeleportIcon { get; private set; } //using same script for Mana as Health bar.
    [field: SerializeField] public IconScript ShadowPullIcon { get; private set; } //using same script for Mana as Health bar.
    [field: SerializeField] public IconScript HealIcon { get; private set; } //using same script for Mana as Health bar.
#endregion
    private void Awake()
    {
        //Setup Animation variables.
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

        #region CallBack Functions
        /*
         * Callback functions used to initiate different controls for the player.
         */
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
#endregion
        SetUpJumpVariables();

        //State Machine setup
        _factory = new PlayerStateFactory(this);
        CurrentState = _factory.Grounded();
        CurrentState.EnterState();
        
        OrbScript = FindObjectOfType<RangedMainOrbScript>();

        //Assigning UI Elements
        HBar = GameObject.Find("HealthBarCanvas").GetComponent<HealthBar>();
        MBar = GameObject.Find("ManaBarCanvas").GetComponent<HealthBar>();
        RangeIcon = GameObject.Find("RangeIcon").GetComponent<IconScript>();
        TeleportIcon = GameObject.Find("TeleportIcon").GetComponent<IconScript>();
        ShadowPullIcon = GameObject.Find("ShadowPullIcon").GetComponent<IconScript>();
        HealIcon = GameObject.Find("HealIcon").GetComponent<IconScript>();
    }

    private void SetUpJumpVariables() //calculates the gravity for the player.
    {
        var timeToApex = _maxJumpTime / 2;
        Gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        InitialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    #region Callbacks Functions
    private void OnMovementInput(InputAction.CallbackContext context) //used to move the player.
    {
        MoveInput = context.ReadValue<Vector2>();
        Moving = MoveInput.x != 0 || MoveInput.y != 0;
    }
    #endregion
    // Start is called before the first frame update
    private void Start() //sets max health/mana
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
        UpdateIconColour();
    }

    private void CheckGrounded() //Checks the character controller is grounded.
    {
        Grounded = CharCont.isGrounded;
    }

    private void ManaRegen() //regenerates Mana until the max value. Only does so when the player is in shadow. 
    {
        if (Lit || !(Mana < MaxMana)) return;
        Mana += 5f * Time.deltaTime;
        Mana = Mathf.Clamp(Mana, 0, MaxMana);
        MBar.UpdateHealthBar(MaxMana, Mana);
    }

    private void UpdateIconColour() //changes Icon colours if the player is above the mana cost.
    {
        TeleportIcon.IconActive = Mana > 25f;
        RangeIcon.IconActive = Mana > 15f;
        HealIcon.IconActive = Mana > 65f;
        ShadowPullIcon.IconActive = Mana > 40f;
    }


    #region animation events

    //Switches the range orb on/off
    public void TriggerStart()
    {
        OrbScript.TriggerStart(true);
    }

    //Fires a new ranged bolt
    public void FireBolt()
    {
        BoltFired = true;
        Mana -= 15f;
        Mana = Mathf.Clamp(Mana, 0, MaxMana);
        var rotation = new Vector3(MainCamera.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
        Instantiate(BoltPrefab, BoltTransform.position, Quaternion.Euler(rotation));
        BoltFired = false;
    }

    //Enables melee colliders
    public void EnableRightMeleeCollider()
    {
        MeleeColliders[0].SetActive(true);
    }
    public void EnableLeftMeleeCollider()
    {
        MeleeColliders[1].SetActive(true);
    }

    //disables melee colliders
    public void DisableMeleeColliders()
    {
        foreach (var obj in MeleeColliders)
        {
            obj.SetActive(false);
        }
    }
    #endregion

    #region Interfaces

    public void HandleHitByLight(int lightValue) //updates the light value for the player
    {
        Lit = lightValue > 25;
    }

    public void HandleDamage(int damageValue) //applied damage to the player
    {
        Health -= damageValue;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        HBar.UpdateHealthBar(MaxHealth, Health);
    }

#endregion

    //enable/disable player controls requied by the unity input system to function. Null reference errors occur otherwise.
    private void OnEnable()
    {
        _playerControls.MainControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.MainControls.Disable();
    }

    private void OnDrawGizmos() //used in debug mode to show the player is grounded or not.
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
