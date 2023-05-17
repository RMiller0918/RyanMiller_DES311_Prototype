using System.Collections;
using UnityEngine;

public class PlayerRangedAttack : PlayerBaseState
{
    public PlayerRangedAttack(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
    }

    public override void EnterState()
    {
        _ctx.Animator.SetBool(_ctx.RangeSetUpHash, true);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if(_ctx.Attacking)
            StartAnimation();
    }

    public override void ExitState()
    {
        _ctx.OrbScript.TriggerStart(false);
        _ctx.Animator.SetBool(_ctx.RangeSetUpHash, false);
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchState()
    {
        if (!_ctx.Aiming)
            SwitchState(_factory.Empty());
    }

    private void StartAnimation()
    {
        _ctx.Animator.SetTrigger(_ctx.RangeFireTriggerHash);
    }
}