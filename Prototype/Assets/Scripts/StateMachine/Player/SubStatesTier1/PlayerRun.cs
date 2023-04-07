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
        //Debug.Log("Run State");
        _isSwitchingState = false;
        InitializeSubState();
        //Debug.Log("Entered Falling State" + "Current Sub State = " + _currentSubState);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if (_isSwitchingState)
        {
            //Debug.Log("Blocking Running after Switch. Switching State: " + _isSwitchingState);
            return;
        }
        Moving();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {
        /*
        if (_ctx.Attacking && !_ctx.Aiming)
            SetSubState(_factory.Attack());
        else if (_ctx.Aiming)
            SetSubState(_factory.RangedAttack());
        else if (_ctx.TeleportSetUp)
            SetSubState(_factory.Teleport());
        else if (_ctx.PullEnemySetUp)
            SetSubState(_factory.PullEnemy());
        else if (_ctx.Healing)
            SetSubState(_factory.Healing());
        else
            SetSubState(_factory.Empty());
        */

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
