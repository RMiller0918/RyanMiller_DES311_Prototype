using UnityEngine;

public class PlayerIdle : PlayerBaseState
{
    public PlayerIdle(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        _isActive = true;
        _isSwitchingState = false;
        InitializeSubState();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if(!_ctx.IsTeleporting)
            StopMoving();
    }

    public override void ExitState()
    {
        _isSwitchingState = false;
        _isActive = false;
    }

    public override void InitializeSubState()
    {
        
        switch (true)
        {
            case var playerCtx when _ctx.Attacking && !_ctx.Aiming && !_ctx.NewAttackRequired:
                SetSubState(_factory.Attack());
                break;
            case var playerCtx when _ctx.Aiming && _ctx.Mana > 15f:
                SetSubState(_factory.RangedAttack());
                break;
            case var playerCtx when _ctx.TeleportSetUp && !_ctx.NewTeleSetUpRequired && _ctx.Mana > 25f:
                SetSubState(_factory.Teleport());
                break;
            case var playerCtx when _ctx.PullEnemySetUp && !_ctx.NewPullRequired && _ctx.Mana > 40f:
                SetSubState(_factory.PullEnemy());
                break;
            case var playerCtx when _ctx.Healing && !_ctx.NewHealRequired && _ctx.Mana > 65f:
                SetSubState(_factory.Healing());
                break;
            default:
                SetSubState(_factory.Empty());
                break;
        }
        
        //SetSubState(_factory.Empty());
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
