using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;

//followed "How to add a Field of View for your Enemies [Unity Tutorial]" - Comp-3 Interactive - https://youtu.be/j1-OyLo77ss
public class FieldOfView : MonoBehaviour
{
    [Range(2,15)]public float Radius;
    [Range(0,360)]public float Angle;

    public GameObject PlayerRef;

    public LayerMask TargetMask;
    public LayerMask ObstacleMask;

    public bool CanSeePlayer;
    public Vector3 PlayerPosition;
    public bool Alert { get; set;}

    private void Awake()
    {
        PlayerRef = GameObject.Find("Player");
    }

    private void Start()
    {
        PlayerRef = GameObject.Find("Player");
        StartCoroutine(FOVRoutine());
    }

    //Co-routine checks the Enemy Field of View 5 times a second.
    private IEnumerator FOVRoutine()
    {
        var delay = 0.2f;
        var wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;
            if (PlayerRef == null)
                PlayerRef = GameObject.Find("Player");
            FieldOfViewCheck();
        }
    }

/*
 * Checks if the player is in view of the enemy character, first by checking the angle of the player to the enemies field of view.
 * If the player is behind an object then they are ignored
 * Also will only register the player as visible if the player is in Light.
 */
    private void FieldOfViewCheck()
    {
        var rangeChecks = Physics.OverlapSphere(transform.position, Radius, TargetMask);

        if (rangeChecks.Length != 0)
        {
            var target = rangeChecks[0].transform;
            if (target == null) return;
            var directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < Angle / 2)
            {
                var distanceToTarget = Vector3.Distance(target.position, transform.position);
                switch (Alert)
                {
                    case true:
                        CanSeePlayer = !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, ObstacleMask);
                        PlayerPosition = target.position;
                        break;
                    default:
                        if (!PlayerRef.GetComponent<PlayerStateMachine>().Lit)
                        {
                            Debug.DrawLine(transform.position, PlayerRef.transform.position, Color.red);
                            return;
                        }
                        CanSeePlayer = !Physics.Raycast(transform.position, directionToTarget, distanceToTarget, ObstacleMask);
                        PlayerPosition = target.position;
                        break;
                }
            }
            else
                CanSeePlayer = false;
        }
        else if (CanSeePlayer)
            CanSeePlayer = false;

        if(CanSeePlayer)
            Debug.DrawLine(transform.position, PlayerRef.transform.position,Color.green);
    }

    private void OnEnable()
    {
        PlayerRef = GameObject.Find("Player");
    }
}
