using UnityEngine;

public class PlayerIdle : PlayerBaseState
{
    public PlayerIdle(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Idle State");
        _isSwitchingState = false;
    }

    public override void UpdateState()
    {
        CheckSwitchState();
    }

    public override void ExitState()
    {
        _isSwitchingState = false;
    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchState()
    {
        if (_ctx.Moving)
        {
            SwitchState(_factory.Walk());
            _isSwitchingState = true;
        }
    }
}
