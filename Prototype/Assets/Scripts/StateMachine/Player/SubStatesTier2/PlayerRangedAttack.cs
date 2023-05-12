using System.Collections;
using UnityEngine;

public class PlayerRangedAttack : PlayerBaseState
{
    private bool _timedOut = false;

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
        if(_ctx.Attacking && !_timedOut)
            StartAnimation();
        if (_ctx.BoltFired && _ctx.Attacking)
            FireBolt();
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
        _timedOut = true;
    }

    private void FireBolt()
    {
        _ctx.RangedAttackCooldown = _ctx.StartCoroutine(Cooldown());
    }

    private void AddBolt()
    {
        var rotation = new Vector3(_ctx.MainCamera.rotation.eulerAngles.x, _ctx.transform.rotation.eulerAngles.y,0);
        Object.Instantiate(_ctx.BoltPrefab, _ctx.BoltTransform.position,Quaternion.Euler(rotation));
        _ctx.BoltFired = false;
    }

    private IEnumerator Cooldown()
    {
        AddBolt();
        yield return new WaitForSeconds(1f);
        _timedOut = false;
        _ctx.StopCoroutine(_ctx.RangedAttackCooldown);
    }
}