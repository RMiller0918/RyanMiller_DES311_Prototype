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
        _isActive = true;
        _isSwitchingState = false;
        InitializeSubState();
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if (_isSwitchingState)
        {
            return;
        }
        if (!_ctx.IsTeleporting)
            Moving();
    }

    public override void ExitState()
    {
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
