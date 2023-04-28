using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : PlayerBaseState, IGravity
{
    private const float fallMultiplier = 2.0f;
    public PlayerJump(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        _isSwitchingState = false;
        SetUpJumpVariables();
        InitializeSubState();
        HandleJump();
    }

    public override void UpdateState()
    { 
        CheckSwitchState();
        HandleGravity();
    }

    public override void ExitState()
    {
        if (_ctx.Jumping)
            _ctx.NewJumpRequired = true;
    }

    public override void InitializeSubState()
    {
        switch (_ctx.Moving)
        {
            case true:
                SetSubState(_factory.Walk());
                break;
            default:
                SetSubState(_factory.Idle());
                break;
        }
    }

    public override void CheckSwitchState()
    {
        if (_ctx.CharCont.isGrounded)
            SwitchState(_factory.Grounded());
    }

    private void HandleJump()
    {
        _ctx.IsJumping = true;
        _ctx.MoveVelocityY = _ctx.InitialJumpVelocity;
        _ctx.AppliedMoveVelocityY = _ctx.InitialJumpVelocity;
    }

    public void HandleGravity()
    {
        var isFalling = _ctx.MoveVelocityY <= 0.0f || !_ctx.Jumping;
        if (isFalling)
        {
            var previousYVelocity = _ctx.MoveVelocityY;
            _ctx.MoveVelocityY += (_ctx.Gravity  * fallMultiplier * Time.deltaTime);
            _ctx.AppliedMoveVelocityY = Mathf.Max((previousYVelocity + _ctx.MoveVelocityY) * .5f, -20.0f);
        }
        else
        {
            var previousYVelocity = _ctx.MoveVelocityY;
            _ctx.MoveVelocityY += (_ctx.Gravity * Time.deltaTime);
            _ctx.AppliedMoveVelocityY = Mathf.Max((previousYVelocity + _ctx.MoveVelocityY) * .5f, -20.0f);
        }
    }

    private void SetUpJumpVariables()
    {
        var timeToApex = _ctx._maxJumpTime / 2;
        _ctx.Gravity = (-2 * _ctx._maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _ctx.InitialJumpVelocity = (2 * _ctx._maxJumpHeight) / timeToApex;
    }
}
