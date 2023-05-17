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
        _ctx.TeleMarker.ResetPosition();
        _ctx.TeleMarker.SetColor(Color.white);
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
        if (_ctx.Attacking && !_ctx.NewAttackRequired)
        {
            SwitchState(_factory.Attack());
            _isSwitchingState = true;
        }
        else if (_ctx.Aiming)
        {
            SwitchState(_factory.RangedAttack());
            _isSwitchingState = true;
        }
        else if (_ctx.TeleportSetUp && !_ctx.NewTeleSetUpRequired && !_ctx.Lit)
        {
            SwitchState(_factory.Teleport());
            _isSwitchingState = true;
        }
        else if (_ctx.PullEnemySetUp && !_ctx.NewPullRequired && !_ctx.Lit)
        {
            Debug.Log("Im trying to enter the pull State");
            SwitchState(_factory.PullEnemy());
            _isSwitchingState = true;
        }
        else if (_ctx.Healing)
        {
            SwitchState(_factory.Healing());
            _isSwitchingState = true;
        }
    }
}
