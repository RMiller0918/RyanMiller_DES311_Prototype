using UnityEngine;
using UnityEngine.AI;
public class EnemyAlerted : EnemyBaseState
{
    private float _timer;
    private float _walkDistance = 3.5f;
    private Vector3 _suspiciousLocation;
    private int _pointsChecked;
    public EnemyAlerted(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
    :base(currentContext, enemyStateFactory)
    { }

    public override void EnterState() //increase the FOV to 300 Degrees.
    {
        _ctx.FOV.Angle = 300f;
        _pointsChecked = 0;
        _ctx.FOV.Alert = true;
        _isActive = true;
        InitializeSubState();
        _ctx.Agent.SetDestination(_ctx.FOV.PlayerRef.transform.position); //Set the destination to the players position.
    }

    public override void UpdateState() 
    {
        Debug.Log($"{this._currentSubState}");
        _ctx.Alert = _pointsChecked < 4;
        CheckVision();
        //SetTarget();
        CheckSwitchState();
    }

    public override void ExitState() //Enemy is no longer alerted.
    {
        _ctx.FOV.Alert = false;
        _ctx.Alert = false;
        _isActive = false;
    }

    public override void InitializeSubState() //initialize movement states.
    {
        if(_ctx.ReadyToMove)
            SetSubState(_factory.Walk());
        if(_ctx.NewTargetNeeded)
            SetSubState(_factory.Idle());

    }

    public override void CheckSwitchState() //switch to unsuspicious or suspicious states.
    {        
        if (_ctx.Alertness >= 0.5f && !_ctx.Alert)
        {
            Debug.Log($"Going from {_factory.Alerted()} to {_factory.Suspicious()}");
            SwitchState(_factory.Suspicious());
        }

        if (_ctx.Alertness == 0 && !_ctx.Alert)
        {
            Debug.Log($"Going from {_factory.Alerted()} to {_factory.Unsuspicious()}");
            SwitchState(_factory.Unsuspicious());
        }
    }

    private void CheckVision() // Check the enemy can see the player. if not check random locations
    {
        // Scale the alert raise by the distance to the player. Turns distance to a 0-1 scale. 0 = furthest away, 1= closest
        var distance = Vector3.Distance(_ctx.FOV.PlayerPosition, _ctx.transform.position);
        distance = distance / _ctx.FOV.Radius;
        distance = 1 - distance;
        switch (_ctx.FOV.CanSeePlayer)
        {
            case false when _pointsChecked < 4:
                SetCheckDestination();
                break;
            case false when _pointsChecked >= 4:
                _ctx.Alert = false;
                break;
            default:
                if (_pointsChecked > 0)
                    _pointsChecked = 0;
                _ctx.Alertness = 1;
                SetPlayerTarget();
                break;
        }
        _ctx.Alertness = Mathf.Clamp(_ctx.Alertness, 0, 1);
        _ctx.AwarenessBar.UpdateHealthBar(1, _ctx.Alertness);
    }

    private void SetPlayerTarget() //sets the target location to the players position
    {
        if (!_ctx.IsGrounded) return;
        if (!_ctx.Agent.isOnNavMesh) return;
        _suspiciousLocation = _ctx.FOV.PlayerPosition;
        var path = new NavMeshPath();
        NavMesh.CalculatePath(_ctx.transform.position, _suspiciousLocation, _ctx.Agent.areaMask, path);
        _ctx.Agent.SetPath(path);
        _ctx.ReadyToMove = Vector3.Distance(_ctx.transform.position, _suspiciousLocation) >= 2f;
        Debug.Log(_ctx.ReadyToMove);
        _ctx.NewTargetNeeded = false;
    }

    private void SetCheckDestination() //gets a random point close to the suspicious location. increases the points checked value.
    {
        if (!_ctx.IsGrounded) return;
        if (!_ctx.Agent.isOnNavMesh) return;
        if (!_ctx.NewTargetNeeded) return;
        Vector3 point;
        if (RandomPoint(_suspiciousLocation, _walkDistance, out point))
        {
            var path = new NavMeshPath();
            NavMesh.CalculatePath(_ctx.transform.position, point, _ctx.Agent.areaMask, path);
            _ctx.Agent.SetPath(path);
            _ctx.ReadyToMove = true;
            _ctx.NewTargetNeeded = false;
            _pointsChecked++;
        }
    }

    private void SetTarget()
    {
        if (!_ctx.IsGrounded) return;
        if (!_ctx.Agent.isOnNavMesh) return;
        if (_ctx.FOV.CanSeePlayer)
        {
            _suspiciousLocation = _ctx.FOV.PlayerPosition;
            var path = new NavMeshPath();
            NavMesh.CalculatePath(_ctx.transform.position, _suspiciousLocation, _ctx.Agent.areaMask, path);
            _ctx.Agent.SetPath(path);
            _ctx.ReadyToMove = Vector3.Distance(_ctx.transform.position, _suspiciousLocation) >= 2f;
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
                _pointsChecked++;
            }
        }
    }
    private bool RandomPoint(Vector3 center, float Range, out Vector3 result) //returns a random point on the navmesh.
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
