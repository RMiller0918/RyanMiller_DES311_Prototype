using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrounded : PlayerBaseState, IGravity
{
    public PlayerGrounded(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState()
    {
        HandleGravity();
        InitializeSubState();
    }

    public override void UpdateState()
    {

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
    }

    public void HandleGravity()
    {

    }
}
