using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealing : PlayerBaseState
{
    public PlayerHealing(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {

    }

    public override void UpdateState()
    {
        ExitState();
    }

    public override void ExitState()
    {

    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchState()
    {

    }
}