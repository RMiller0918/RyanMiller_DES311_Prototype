using UnityEngine;

public class PlayerWalk : PlayerBaseState, IMoveable
{
    private int _moveSpeed;

    public PlayerWalk(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _moveSpeed = 4;
    }

    public override void EnterState()
    {
        Debug.Log("Walk State");
        _isSwitchingState = false;
        InitializeSubState();
        Debug.Log("Entered Walk State" + "Current Sub State = " + _currentSubState);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if (_isSwitchingState)
        {
            //Debug.Log("Blocking Walking after Switch. Switching State: " + _isSwitchingState);
            return;
        }
        Moving();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        switch (true)
        {
            case var playerCtx when _ctx.Attacking && !_ctx.Aiming:
                SetSubState(_factory.Attack());
                break;
            case var playerCtx when _ctx.Aiming:
                SetSubState(_factory.RangedAttack());
                break;
            case var playerCtx when _ctx.TeleportSetUp:
                SetSubState(_factory.Teleport());
                break;
            case var playerCtx when _ctx.PullEnemySetUp:
                SetSubState(_factory.PullEnemy());
                break;
            case var playerCtx when _ctx.Healing:
                SetSubState(_factory.Healing());
                break;
            default:
                SetSubState(_factory.Empty());
                break;
        }
    }

    public override void CheckSwitchState()
    {
        if (!_ctx.Moving)
        {
            SwitchState(_factory.Idle());
            _isSwitchingState = true;
        }
        else if (_ctx.Sprinting)
        {
            SwitchState(_factory.Run()); 
            _isSwitchingState = true;
        }
    }

    public void Moving()
    {
        var move = (_ctx.transform.right * _ctx.MoveInput.x + _ctx.transform.forward * _ctx.MoveInput.y) * _moveSpeed;
        _ctx.MoveVelocityX = move.x;
        _ctx.MoveVelocityZ = move.z;
    }
}
