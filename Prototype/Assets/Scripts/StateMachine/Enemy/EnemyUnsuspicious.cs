using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class EnemyUnsuspicious : EnemyBaseState
{
    private float _walkDistance = 3.5f;
    public EnemyUnsuspicious(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory)
    {
    }

    public override void EnterState() //Set the FOV to 90 degrees, initialize movement states
    {
        _ctx.FOV.Angle = 90f;
        _isActive = true;
        Debug.Log($"Root: {_currentSuperState} SubState_1: {this}");
        InitializeSubState();
    }

    public override void UpdateState() //Check if a new target location is needed. Check the enemy vision and if a state switch is needed.
    {
        if(_ctx.NewTargetNeeded)
            SetTarget();
        CheckVision();
        CheckSwitchState();
    }

    public override void ExitState()
    {
        _isActive = false;
    }

    public override void InitializeSubState() //initialize the movement State.
    {
        SetSubState(_ctx.ReadyToMove ? _factory.Walk() : _factory.Idle());
    }

    public override void CheckSwitchState() //Switch between Alerted and Suspicious when conditions are met.
    {
        if (_ctx.Alertness >= 0.5f && !_ctx.Alert)
        {
            //Debug.Log($"Going from {_factory.Unsuspicious()} to {_factory.Suspicious()}");
            SwitchState(_factory.Suspicious());
        }

        if (_ctx.Alertness >= 0.5f && _ctx.Alert)
        {
            //Debug.Log($"Going from {_factory.Unsuspicious()} to {_factory.Alerted()}");
            SwitchState(_factory.Alerted());
        }
    }

    private void SetTarget() //returns a new random point on the navmesh within the Walk Distance. Calculates a path to the target.
    {
        if (!_ctx.IsGrounded) return;
        if (!_ctx.Agent.isOnNavMesh) return;
        Vector3 point;
        if (RandomPoint(_ctx.SpawnPosition, _walkDistance, out point))
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(_ctx.transform.position, point, _ctx.Agent.areaMask, path);
            _ctx.Agent.SetPath(path);
            _ctx.ReadyToMove = true;
            _ctx.NewTargetNeeded = false;
        }
    }

    private void CheckVision() //Check if the enemy can see the player and updates the Awareness meter based on the distance. Scales the distance value to be between 0(furthest)-1(closest).
    {
        var distance = Vector3.Distance(_ctx.FOV.PlayerPosition, _ctx.transform.position);
        distance = distance / _ctx.FOV.Radius;
        distance = 1 - distance;
        _ctx.Alertness += _ctx.FOV.CanSeePlayer ? distance * Time.deltaTime: -Time.deltaTime;
        _ctx.Alertness = Mathf.Clamp(_ctx.Alertness, 0, 1);
        _ctx.AwarenessBar.UpdateHealthBar(1, _ctx.Alertness);
    }

    private bool RandomPoint(Vector3 center, float Range, out Vector3 result) //Returns a random point. 
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
