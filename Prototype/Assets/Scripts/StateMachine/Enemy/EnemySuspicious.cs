using UnityEngine;
using UnityEngine.AI;

public class EnemySuspicious : EnemyBaseState
{
    private Vector3 _suspiciousLocation;
    private float _timer;
    private float _walkDistance = 3.5f;
    public EnemySuspicious(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory)
    {
    }

    public override void EnterState() //increase the FOV to 120 degrees.
    {
        _ctx.FOV.Angle = 120f;
        InitializeSubState();
    }

    public override void UpdateState()
    {
        SetTarget();
        CheckVision();
        CheckSwitchState();
    }

    public override void ExitState() { }

    public override void InitializeSubState() //initialize movement states
    {
        SetSubState(_ctx.ReadyToMove ? _factory.Walk() : _factory.Idle());
    }

    public override void CheckSwitchState() //switch between alert and unsuspicious states.
    {
        if (_ctx.Alertness >= 0.5f && _ctx.Alert)
        {
            //Debug.Log($"Going from {_factory.Suspicious()} to {_factory.Alerted()}");
            SwitchState(_factory.Alerted());
        }

        if (_ctx.Alertness == 0 && !_ctx.Alert)
        {
            //Debug.Log($"Going from {_factory.Suspicious()} to {_factory.Unsuspicious()}");
            SwitchState(_factory.Unsuspicious());
        }
    }

    private void CheckVision() //Check if the enemy can see the player. Decrease the Alert meter after a time. Update the awareness meter
    {
        var distance = Vector3.Distance(_ctx.FOV.PlayerPosition, _ctx.transform.position);
        distance = distance / _ctx.FOV.Radius;
        distance = 1 - distance;
        switch (_ctx.FOV.CanSeePlayer)
        {
            case false when _timer <= 2f:
                _timer += Time.deltaTime;
                break;
            case false when _timer >= 2f:
                _ctx.Alertness -= 0.1f * Time.deltaTime;
                break;
            default:
                _timer = 0;
                _ctx.Alertness += distance * Time.deltaTime;
                if (_ctx.Alertness >= 1)
                    _ctx.Alert = true;
                break;
        }
        _ctx.Alertness = Mathf.Clamp(_ctx.Alertness, 0, 1);
        _ctx.AwarenessBar.UpdateHealthBar(1, _ctx.Alertness);
    }

    private void SetTarget() //Set a new target location. If the enemy can still see the player set the target location to be the players current location if not chose a random point on the navmesh.
    {
        if (!_ctx.IsGrounded) return;
        if (!_ctx.Agent.isOnNavMesh) return;
        if (_ctx.FOV.CanSeePlayer)
        {
            _suspiciousLocation = _ctx.FOV.PlayerPosition;
            var path = new NavMeshPath();
            NavMesh.CalculatePath(_ctx.transform.position, _suspiciousLocation, _ctx.Agent.areaMask, path);
            _ctx.Agent.SetPath(path);
            _ctx.ReadyToMove = true;
            _ctx.NewTargetNeeded = false;
        }
        else
        {
            if (!_ctx.NewTargetNeeded) return;
            Vector3 point;
            if (RandomPoint(_suspiciousLocation, _walkDistance, out point))
            {
                var path = new NavMeshPath();
                NavMesh.CalculatePath(_ctx.transform.position, point, _ctx.Agent.areaMask, path);
                _ctx.Agent.SetPath(path);
                _ctx.ReadyToMove = true;
                _ctx.NewTargetNeeded = false;
            }
        }
    }
    private bool RandomPoint(Vector3 center, float Range, out Vector3 result) //return a random point on the navmesh.
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
