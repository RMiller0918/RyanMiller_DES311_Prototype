using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerStateMachine : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerControls _playerControls;
    [field: SerializeField] public PlayerBaseState CurrentState { get; set; }
    [SerializeField] private PlayerStateFactory _factory;
    [field: SerializeField] public CharacterController CharCont { get; private set; }

    [field: Header("Player Stats")]
    [field: SerializeField] [field: Range(100,500)] public int Health { get; private set; }

    [field: Header("Animation")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public int AnimationHash { get; private set; }
    [field: SerializeField] public Vector2 MoveInput { get; private set; }

    [field: Header("State Booleans")]
    [field: SerializeField] public bool Moving { get; private set; }
    [field: SerializeField] public bool Sprinting { get; private set; }
    [field: SerializeField] public bool Crouching { get; private set; }
    [field: SerializeField] public bool Grounded { get; private set; }
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        CharCont = GetComponent<CharacterController>();

        _playerControls = new PlayerControls();

        _playerControls.MainControls.Move.performed += OnMovementInput;
        _playerControls.MainControls.Move.started += OnMovementInput;
        _playerControls.MainControls.Move.canceled += OnMovementInput;
        _playerControls.MainControls.Sprint.performed += ctx => Sprinting = ctx.ReadValueAsButton();

        _factory = new PlayerStateFactory(this);
        CurrentState = _factory.Grounded();
        CurrentState.EnterState();
    }

    #region Callbacks
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
    }

    private void OnEnable()
    {
        _playerControls.MainControls.Enable();
    }

    private void OnDisable()
    {
        _playerControls.MainControls.Disable();
    }
}
