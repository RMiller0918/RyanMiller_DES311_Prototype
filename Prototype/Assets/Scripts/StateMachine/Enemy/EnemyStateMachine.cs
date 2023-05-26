using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyStateMachine : MonoBehaviour, IDamageable, ILightable
{
    [SerializeField] private EnemyScriptableObject _enemyData;
    private EnemyStateFactory _factory;
    [field: SerializeField] public EnemyBaseState CurrentState { get; set; }
    [field: SerializeField] public int MaxHealth { get; private set; }
    [SerializeField] private int _health;
    [field: SerializeField] public CharacterController CharCont { get; private set; }

    [field: SerializeField] public bool IsGrounded { get; private set; }

    [field: Header("Gravity")]
    [field: SerializeField] public float Gravity { get; set; }
    [field: SerializeField] private LayerMask _mask;

    //Enemies do not jump, but the variables under the Jumping Header are required to calculate character gravity
    [field: Header("Jumping")]
    [field: SerializeField] public float InitialJumpVelocity { get; set; }
    [field: SerializeField] [field: Range(2, 4)] public float _maxJumpHeight { get; private set; }
    [field: SerializeField] [field: Range(0.5f, 1)] public float _maxJumpTime { get; private set; }

    [field: Header("MovementVectors")]

    public Vector3 MoveVelocity;
    [field: SerializeField] public float MoveVelocityX { get => MoveVelocity.x; set => MoveVelocity.x = value; }
    [field: SerializeField] public float MoveVelocityY { get => MoveVelocity.y; set => MoveVelocity.y = value; }
    [field: SerializeField] public float MoveVelocityZ { get => MoveVelocity.z; set => MoveVelocity.z = value; }

    [field: SerializeField] public Vector3 AppliedMoveVelocity;
    [field: SerializeField] public float AppliedMoveVelocityY { get => AppliedMoveVelocity.y; set => AppliedMoveVelocity.y = value; }

    [field:Header("Lighting")]
    [field: SerializeField] public bool Lit { get; private set; }

    [Header("Health Bar")]
    [SerializeField] private HealthBar _healthBar;

    [field: Header("Alertness")] 
    [field: SerializeField] public FieldOfView FOV { get; private set; }
    [field: SerializeField] public float Alertness { get; set; }
    [field: SerializeField] public bool Alert { get; set; }
    [field: SerializeField] public Vector3 SuspiciousLocation { get; set; }
    [field: SerializeField] public HealthBar AwarenessBar { get; private set; }

    [field: Header("Navigation")]
    [field: SerializeField] public Vector3 SpawnPosition { get; private set; }
    [field: SerializeField] public NavMeshAgent Agent { get; private set; }
    [field: SerializeField] public bool NewTargetNeeded { get; set; }
    [field: SerializeField] public bool ReadyToMove { get; set; }

    [field: SerializeField] public List<Vector3> Waypoints { get; set; }

    [field: Header("Animation")]
    [field: SerializeField] public Animator Animator { get; private set; }
    [field: SerializeField] public int SwingHash { get; private set; }


    [field: Header("Status")]
    [field: SerializeField] public bool Stunned { get; set; }

    private void Awake()
    {
        FOV = GetComponent<FieldOfView>();
        _healthBar = GetComponentInChildren<HealthBar>();
        CharCont = GetComponent<CharacterController>();
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
        SwingHash = Animator.StringToHash("Swing");
        _factory = new EnemyStateFactory(this);
        CurrentState = _factory.Grounded();
        CurrentState.EnterState();
    }

    void Start()
    {
        MaxHealth = _enemyData.Health;
        _health = MaxHealth;
        SpawnPosition = transform.position;
        _mask = LayerMask.GetMask("Enemy");
    }

    void Update() //handle moving the character controller when it is active.
    {
        //Debug.Log(CurrentState);
        CurrentState.UpdateStates();
        AppliedMoveVelocity.x = MoveVelocity.x;
        AppliedMoveVelocity.z = MoveVelocity.z;
        CharCont.Move(AppliedMoveVelocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        GroundedCheck();
        if (_health <= 0)
            Destroy(this.gameObject);
    }

    private void GroundedCheck() //Uses sphere cast to check if the enemy is grounded. Needed since Character controller is inactive when the enemy is grounded.
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        IsGrounded = Physics.SphereCast(ray, 0.5f, 1.2f, ~_mask);
        
    }

    public void HandleDamage(int damageValue) //Applies damage to the enemy and updates health bar
    {
        _health -= damageValue;
        _healthBar.UpdateHealthBar(MaxHealth, _health);
        Alert = true;
        Alertness = 100f;
    }

    public void HandleHitByLight(int lightValue) //updates the light value
    {
        //Debug.Log(lightValue);
        Lit = lightValue > 25;
    }

    private void OnTriggerEnter(Collider other) //Calculates a new path if the enemy walks into an obstacle or another enemy. Alerted to the player presence if walking into the player.
    {
        if (other.tag == "Enemy" || other.tag == "Environment")
        {
            if (!Agent.isOnNavMesh) return;
            Agent.CalculatePath(Agent.destination, Agent.path);
        }
        if (other.tag == "Player" && !Alert)
            Alert = true;
    }

    private void OnTriggerExit(Collider other) //calculates a new path if an enemy leaves the trigger box. Alerted to the player presence if the player leaves the trigger. 
    {
        if (other.tag == "Enemy" || other.tag == "Environment")
        {
            if (!Agent.isOnNavMesh) return;
            Agent.CalculatePath(Agent.destination, Agent.path);
        }
        if (other.tag == "Player" && !Alert)
            Alert = true;
    }
    private void OnDrawGizmos() //shows the grounded state in debug mode.
    {
        Gizmos.color = IsGrounded ? Color.green : Color.red;
        var sphVect = transform.position + (Vector3.down * 1.2f);
        Gizmos.DrawWireSphere(sphVect, 0.5f);
    }
}
