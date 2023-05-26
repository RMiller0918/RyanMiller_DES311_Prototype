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
        _isActive = true;
        _isSwitchingState = false;
        InitializeSubState();
    }

    public override void UpdateState() //player will only move in this state if they aren't already teleporting.
    {
        CheckSwitchState();
        if (_isSwitchingState)
        {
            return;
        }

        if (!_ctx.IsTeleporting)
        {
            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 95, 5f * Time.deltaTime);
            Moving();
        }
    }

    public override void ExitState() //return FOV to 90
    {
        Camera.main.fieldOfView = 90;
        _isActive = false;
    }

    public override void InitializeSubState() //Initialize the ability substates.
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
        
    }

    public override void CheckSwitchState() //Switch between idle and walking states
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

    public void Moving() //Apply the move velocity vector based on Input and camera forward.
    {
        var move = (_ctx.transform.right * _ctx.MoveInput.x + _ctx.transform.forward * _ctx.MoveInput.y) * _moveSpeed;
        _ctx.MoveVelocityX = move.x;
        _ctx.MoveVelocityZ = move.z;
    }
}
