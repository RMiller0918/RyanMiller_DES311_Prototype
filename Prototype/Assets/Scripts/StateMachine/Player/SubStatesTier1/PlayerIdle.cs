using UnityEngine;

public class PlayerIdle : PlayerBaseState
{
    public PlayerIdle(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        _isSwitchingState = false;
        InitializeSubState();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        StopMoving();
    }

    public override void ExitState()
    {
        _isSwitchingState = false;
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
        if (_ctx.Moving)
        {
            SwitchState(_factory.Walk());
            _isSwitchingState = true;
        }
    }

    private void StopMoving()
    {
        if (_ctx.MoveVelocityX == 0 && _ctx.MoveVelocityZ == 0) return;
        _ctx.MoveVelocityX = 0;
        _ctx.MoveVelocityZ = 0;
    }
}
