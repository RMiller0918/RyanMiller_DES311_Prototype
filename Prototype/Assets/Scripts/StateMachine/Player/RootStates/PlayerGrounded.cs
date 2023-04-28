using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : PlayerBaseState, IGravity
{
    private float _groundedGravity = -0.5f;
    public PlayerGrounded(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        _ctx.IsJumping = false;
        SetUpGravity();
        InitializeSubState();
        HandleGravity();
        //Debug.Log("Entered Grounded State" + "Current Sub State = " + _currentSubState);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void ExitState()
    {
        
    }

    public override void InitializeSubState()
    {
        switch (_ctx.Moving)
        {
            case true when !_ctx.Sprinting:
                SetSubState(_factory.Walk());
                break;
            case true when _ctx.Sprinting:
                SetSubState(_factory.Run());
                break;
            default:
                SetSubState(_factory.Idle());
                break;
        }
    }

    public override void CheckSwitchState()
    {
        if (_ctx.Jumping && !_ctx.NewJumpRequired)
        {
            SwitchState(_factory.Jump());
        }
        if(!_ctx.CharCont.isGrounded)
            SwitchState(_factory.Falling());
    }

    public void HandleGravity()
    {
        _ctx.MoveVelocityY = _ctx.Gravity;
        _ctx.AppliedMoveVelocityY = _ctx.Gravity;
    }

    private void SetUpGravity()
    {
        _ctx.Gravity = _groundedGravity;
    }
}
