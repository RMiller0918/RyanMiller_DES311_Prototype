using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPullEnemy : PlayerBaseState
{
    private bool _finished, _cancelled;
    public PlayerPullEnemy(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _finished = false;
        _cancelled = false;
    }

    public override void UpdateState()
    {

    }

    public override void ExitState()
    {
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchState()
    {
        if(_finished || _cancelled)
            SwitchState(_factory.Empty());
    }
}