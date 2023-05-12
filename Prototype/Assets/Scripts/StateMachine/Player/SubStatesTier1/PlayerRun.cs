using UnityEngine;

public class PlayerRun : PlayerBaseState, IMoveable
{
    private int _moveSpeed;

    public PlayerRun(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _moveSpeed = 7;
    }

    public override void EnterState()
    {
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
        Moving();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        switch (true)
        {
            case var playerCtx when _ctx.Attacking && !_ctx.Aiming && !_ctx.NewAttackRequired:
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
        if (!_ctx.Sprinting && _ctx.Moving)
        {
            SwitchState(_factory.Walk());
            _isSwitchingState = true;
        }
        else if (!_ctx.Moving)
        {
            SwitchState(_factory.Idle());
            _isSwitchingState = true;
        }
    }

    public void Moving()
    {
        if(_isSwitchingState)
            Debug.Log("Moving After Switching");
        var move = (_ctx.transform.right * _ctx.MoveInput.x + _ctx.transform.forward * _ctx.MoveInput.y) * _moveSpeed;
        _ctx.MoveVelocityX = move.x;
        _ctx.MoveVelocityZ = move.z;
    }
}
