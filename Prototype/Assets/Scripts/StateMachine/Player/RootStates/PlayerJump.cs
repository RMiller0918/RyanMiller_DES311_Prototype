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

    public override void EnterState() //Set up the jump variables and apply the initial jump.
    {
        _isActive = true;
        _isSwitchingState = false;
        SetUpJumpVariables();
        InitializeSubState();
        HandleJump();
    }

    public override void UpdateState() //Handle the gravity. 
    { 
        CheckSwitchState();
        HandleGravity();
    }

    public override void ExitState() //if the player is holding space make the player release the button in order to jump again.
    {
        if (_ctx.Jumping)
            _ctx.NewJumpRequired = true;
        _isActive = false;
    }

    public override void InitializeSubState() //Initialize movement Substates.
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

    public override void CheckSwitchState() //Switch to the grounded state
    {
        if (_ctx.CharCont.isGrounded)
            SwitchState(_factory.Grounded());
    }

    private void HandleJump() //Applies the initial jump velocity to the applied movement vector.
    {
        _ctx.IsJumping = true;
        _ctx.MoveVelocityY = _ctx.InitialJumpVelocity;
        _ctx.AppliedMoveVelocityY = _ctx.InitialJumpVelocity;
    }

    public void HandleGravity() //properly calculates the correct point in the jump based on velocity the previous frame. //Gravity decreases every frame. 
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

    private void SetUpJumpVariables() //calculates gravity and initial jump velocity. 
    {
        var timeToApex = _ctx._maxJumpTime / 2;
        _ctx.Gravity = (-2 * _ctx._maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _ctx.InitialJumpVelocity = (2 * _ctx._maxJumpHeight) / timeToApex;
    }
}
