using UnityEngine;
using UnityEngine.AI;

public class EnemyGrounded : EnemyBaseState, IGravity
{
    private float _groundedGravity = .6f;
    public EnemyGrounded(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState() //when the enemy is grounded, disable the character controller and enable the NavMeshAgent
    {
        _ctx.CharCont.enabled = false;
        _ctx.Agent.enabled = true;
        _isActive = true;
        SetupGravity(); 
        InitializeSubState();
    }

    public override void UpdateState() //check for state switching.
    {
        HandleGravity();
        CheckSwitchState();
    }

    public override void ExitState()
    {
        _isActive = false;
    }

    public override void InitializeSubState() //Initialize the suspicion states
    {
        switch (_ctx.Alertness)
        {
            case < 0.5f:
                SetSubState(_factory.Unsuspicious());
                break;
            case >= 0.5f:
                SetSubState(_factory.Suspicious());
                break;
            default:
            {
                if (_ctx.Alert)
                    SetSubState(_factory.Alerted());
                break;
            }
        }
    }

    public override void CheckSwitchState() //switch to falling state if the character is not grounded.
    {
        if(!_ctx.IsGrounded && _ctx.Agent.nextOffMeshLinkData.linkType != OffMeshLinkType.LinkTypeJumpAcross)
            SwitchState(_factory.Falling());
    }

    public void HandleGravity() //apply grounded gravity
    {
        _ctx.MoveVelocityY = _ctx.Gravity;
        _ctx.AppliedMoveVelocityY = _ctx.Gravity;
    }

    private void SetupGravity() //set gravity value.
    {
        _ctx.Gravity = _groundedGravity;
    }
}
