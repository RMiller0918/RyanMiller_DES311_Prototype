using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PlayerDefault : PlayerBaseState
{
    public PlayerDefault(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isSwitchingState = false;
    }

    public override void EnterState()
    {
        _isSwitchingState = false;
        Debug.Log("Empty State. Current Super State =  " + _currentSuperState);
    }

    public override void UpdateState()
    {
        if (_isSwitchingState) return;
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
        if(_ctx.Attacking && !_ctx.NewAttackRequired)
            SwitchState(_factory.Attack());
        else if (_ctx.Aiming)
        {
            SwitchState(_factory.RangedAttack());
            _isSwitchingState = true;
        }
        else if(_ctx.TeleportSetUp)
            SwitchState(_factory.Teleport());
        else if(_ctx.PullEnemySetUp)
            SwitchState(_factory.PullEnemy());
        else if(_ctx.Healing)
            SwitchState(_factory.Healing());
    }
}
