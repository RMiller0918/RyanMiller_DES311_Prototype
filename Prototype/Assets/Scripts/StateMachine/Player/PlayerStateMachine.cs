using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] private PlayerControls _playerControls;
    [field: SerializeField] public PlayerBaseState CurrentState { get; set; }
    [SerializeField] private CharacterController _characterController;

    [field: Header("Player Stats")]
    [field: SerializeField] [field: Range(100,500)] public int Health { get; private set; }

    [field: Header("Animation")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public int AnimationHash { get; private set; }
    [field: SerializeField] public GameObject Camera { get; private set; }

    [SerializeField] private Vector3 _moveVector;

    private void Awake()
    {
        _playerControls = new PlayerControls();

        _playerControls.MainControls.Move.performed += MoveCallback;
        _playerControls.MainControls.Move.started += MoveCallback;
        _playerControls.MainControls.Move.canceled += MoveCallback;
    }

    private void MoveCallback(InputAction.CallbackContext context)
    {
        _moveVector = context.ReadValue<Vector3>();
    }

    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        _characterController.Move(_moveVector * Time.deltaTime);
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
