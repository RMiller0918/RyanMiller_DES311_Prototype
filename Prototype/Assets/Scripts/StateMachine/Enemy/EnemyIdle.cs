using Unity.VisualScripting;
using UnityEngine;

public class EnemyIdle : EnemyBaseState
{
    private float _timer;
    private int _timerTarget;

    public EnemyIdle(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory)
    {

    }

    public override void EnterState()
    {
        _timerTarget = Random.Range(1, 2);
        _timer = 0f;
        _isActive = true;
        _ctx.Agent.speed = 0;
        InitializeSubState();
    }

    public override void UpdateState() //waits for a new target location before attempting to move. If the player is not visible, the enemy will wait until the target time has been reached before moving.
    {
        switch (_ctx.Alert)
        {
            case true when _ctx.FOV.CanSeePlayer:
                if (Vector3.Distance(_ctx.transform.position, _ctx.FOV.PlayerPosition) > 1f)
                {
                    //Debug.Log("I can see the player, checking if in range");
                    _ctx.NewTargetNeeded = true;
                    CheckSwitchState();
                }
                break;
            case true when !_ctx.FOV.CanSeePlayer:
                //Debug.Log($"I can't see the player, running timer up to {_timerTarget}");
                _timer += Time.deltaTime;
                if (_timer >= _timerTarget)
                {
                    _ctx.NewTargetNeeded = true;
                    CheckSwitchState();
                }
                break;
            default:
                //Debug.Log($"I'm patrolling, running timer up to {_timerTarget}");
                _timer += Time.deltaTime;
                if (_timer >= _timerTarget)
                {
                    _ctx.NewTargetNeeded = true;
                    CheckSwitchState();
                }
                break;
        }

    }

    public override void ExitState()
    {
        _isActive = false;
    }

    public override void InitializeSubState() //initialize the attack state.
    {
        SetSubState(_factory.Attack());
    }

    public override void CheckSwitchState() //switch to walking
    {
        if(_ctx.ReadyToMove)
            SwitchState(_factory.Walk());
    }
}
