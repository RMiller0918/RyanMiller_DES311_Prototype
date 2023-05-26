using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFalling : PlayerBaseState
{
    private float _fallTime;
    public PlayerFalling(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState() //initializes gravity and substates. sets up gravity variables.
    {
        _fallTime = 0;
        _isActive = true;
        _ctx.IsJumping = false;
        _isSwitchingState = false;
        InitializeSubState();
        SetUpJumpVariables();
    }

    public override void UpdateState() //handles gravity and switch states
    {
        CheckSwitchState();
        if (_isSwitchingState) return;
        HandleGravity();
        _fallTime += Time.deltaTime;
    }

    public override void ExitState() //if the player has been falling for 2 seconds, damage the player for 100, if over 10 seconds kill the player character.
    {
        if(_fallTime > 2f)
            _ctx.HandleDamage(100);
        if(_fallTime > 10f)
            _ctx.HandleDamage(_ctx.MaxHealth);
        _isActive = false;
    }

    public override void InitializeSubState() //initialize movement substates
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

    public override void CheckSwitchState() //switch to grounded state
    {
        if (_ctx.CharCont.isGrounded)
        {
            SwitchState(_factory.Grounded());
            _isSwitchingState = true;
        }
    }
    public void HandleGravity() //apply the gravity variable over time.
    {
        var previousYVelocity = _ctx.MoveVelocityY;
        _ctx.MoveVelocityY += (_ctx.Gravity + Time.deltaTime);
        _ctx.AppliedMoveVelocityY = Mathf.Max((previousYVelocity + _ctx.MoveVelocityY) * .5f, -20.0f);
    }

    private void SetUpJumpVariables() //sets up the gravity variable.
    {
        var timeToApex = _ctx._maxJumpTime / 2;
        _ctx.Gravity = (-2 * _ctx._maxJumpHeight) / Mathf.Pow(timeToApex, 2);
    }
}