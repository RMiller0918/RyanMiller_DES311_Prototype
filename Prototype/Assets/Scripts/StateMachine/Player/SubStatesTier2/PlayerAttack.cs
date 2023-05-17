using System.Collections;
using UnityEngine;

public class PlayerAttack : PlayerBaseState
{
    private Coroutine _currentMeleeResetRoutine;

    public PlayerAttack(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _isActive = true;
        Debug.Log(_ctx.MeleeCount);
        if (_ctx.MeleeCount > 2)
        {
            _ctx.MeleeCount = 0;
            _ctx.Animator.SetInteger(_ctx.MeleeCountHash, _ctx.MeleeCount);
        }
        else
        {
            _ctx.MeleeCount += 1;
            _ctx.Animator.SetInteger(_ctx.MeleeCountHash, _ctx.MeleeCount);
        }
        _ctx.Animator.SetTrigger(_ctx.MeleeHash);
    }

    public override void UpdateState()
    {
        if (_ctx.MeleeCount < 2 && _currentMeleeResetRoutine != null)
            _ctx.StopCoroutine(_currentMeleeResetRoutine); 
        CheckSwitchState();
    }

    public override void ExitState()
    {
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
        _isActive = false;
    }

    public override void InitializeSubState() 
    { }

    public override void CheckSwitchState()
    {
        _currentMeleeResetRoutine = _ctx.StartCoroutine(MeleeResetRoutine());
        SwitchState(_factory.Empty());
    }

    private IEnumerator MeleeResetRoutine()
    {
        yield return new WaitForSeconds(1f);
        _ctx.MeleeCount = 0;
        _ctx.Animator.SetInteger(_ctx.MeleeCountHash, _ctx.MeleeCount);
    }
}
