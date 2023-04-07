using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFalling : PlayerBaseState
{
    public PlayerFalling(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        _ctx.IsJumping = false;
        _isSwitchingState = false;
        InitializeSubState();
        SetUpJumpVariables();
        //Debug.Log("Entered Falling State" + "Current Sub State = " + _currentSubState);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if (_isSwitchingState) return;
        HandleGravity();
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
            case false:
                SetSubState(_factory.Idle());
                break;
        }
    }

    public override void CheckSwitchState()
    {
        if (_ctx.CharCont.isGrounded)
        {
            SwitchState(_factory.Grounded());
            _isSwitchingState = true;
        }
    }
    public void HandleGravity()
    {
        var previousYVelocity = _ctx.MoveVelocityY;
        _ctx.MoveVelocityY += (_ctx.Gravity + Time.deltaTime);
        _ctx.AppliedMoveVelocityY = Mathf.Max((previousYVelocity + _ctx.MoveVelocityY) * .5f, -20.0f);
    }

    private void SetUpJumpVariables()
    {
        var timeToApex = _ctx._maxJumpTime / 2;
        _ctx.Gravity = (-2 * _ctx._maxJumpHeight) / Mathf.Pow(timeToApex, 2);
    }
}