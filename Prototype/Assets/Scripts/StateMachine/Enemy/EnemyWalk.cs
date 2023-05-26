using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

public class EnemyWalk : EnemyBaseState
{
    private float _walkSpeed = 4;
    private float _initialDistance;
    private float _remainingDistance;
    public EnemyWalk(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory) { }

    public override void EnterState() //Calculate the initial distance to the target location when entering this state.
    {
        _isActive = true;
        _ctx.Agent.speed = _walkSpeed;
        Debug.Log("Enemy Walk State");
        _initialDistance = Mathf.Round(Vector3.Distance(_ctx.transform.position, _ctx.Agent.destination) * 100);
        _initialDistance /= 100f;
        InitializeSubState();
    }

    public override void UpdateState() //change walk speed based on remaining distance to target.
    {
        if (!_ctx.IsGrounded) return;
        if (_ctx.Agent.isOnNavMesh)
        {
            _ctx.Agent.speed = _ctx.Agent.remainingDistance > 4f ? 6f : 4f;
        }
        _ctx.Agent.avoidancePriority = (int)UpdatePriority(); //update the avoidance priority.
        CheckPosition();
        CheckSwitchState();
    }

    public override void ExitState()
    {
        _isActive = false;
    }

    public override void InitializeSubState()
    {
        SetSubState(_factory.Attack());
    }

    public override void CheckSwitchState()
    {
        if(_ctx.NewTargetNeeded)
            SwitchState(_factory.Idle());
    }
    private void CheckPosition() //check distance to target position. If the target is the player, stop moving when in range. otherwise move until the enemy is within the stopping distance of their target location. 
    {
        if (!_ctx.IsGrounded) return;
        if (!_ctx.Agent.isOnNavMesh) return;
        //Debug.Log("Checking My Position");
        switch (_ctx.Alert)
        {
            case true when _ctx.FOV.CanSeePlayer:
                //Debug.Log("I can see the player, checking if in range");
                if (!_ctx.IsGrounded) return;
                if (!_ctx.Agent.isOnNavMesh) return;
                if (_ctx.Agent.remainingDistance <= 1.5f)
                {
                    _ctx.NewTargetNeeded = true;
                    _ctx.ReadyToMove = false;
                }
                break;
            case true when !_ctx.FOV.CanSeePlayer:
                //Debug.Log("I can see the player, checking im at the last point");
                if (!_ctx.IsGrounded) return;
                if (!_ctx.Agent.isOnNavMesh) return;
                if (_ctx.Agent.remainingDistance <= _ctx.Agent.stoppingDistance)
                {
                    _ctx.NewTargetNeeded = true;
                    _ctx.ReadyToMove = false;
                }
                break;
            default:
                //Debug.Log("I can't see the player, I'm just walking around unalerted.");
                if (!_ctx.IsGrounded) return;
                if (!_ctx.Agent.isOnNavMesh) return;
                if (_ctx.Agent.remainingDistance <= _ctx.Agent.stoppingDistance)
                {
                    _ctx.NewTargetNeeded = true;
                    _ctx.ReadyToMove = false;
                }
                break;
        }
    }

    private float UpdatePriority() //calculates a new priority value for this enemy based on how far they are through their navmesh path. Helps to stop enemies causing each other to stop moving.
    {
        _remainingDistance = Mathf.Round(Vector3.Distance(_ctx.transform.position, _ctx.Agent.destination) * 100f);
        _remainingDistance /= 100f;
        var priority = (_remainingDistance / _initialDistance) * 100f;
        priority = Mathf.Round(priority);
        priority = 100 - priority;
        return priority;
    }

    private bool RandomPoint(Vector3 center, float Range, out Vector3 result) 
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * Range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = _ctx.SpawnPosition;
        return false;
    }
}
