using System.Collections;
using UnityEngine;

public class PlayerRangedAttack : PlayerBaseState
{
    private bool _timedOut = false;
    private float _timer;
    public PlayerRangedAttack(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        _ctx.Animator.SetBool(_ctx.RangeSetUpHash,true);
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if (_ctx.Attacking && _ctx.Aiming)
            FireBolt();
    }

    public override void ExitState()
    {
        _ctx.TriggerStart();
        _ctx.Animator.SetBool(_ctx.RangeSetUpHash, false);
    }

    public override void InitializeSubState(){}

    public override void CheckSwitchState()
    {
        if(!_ctx.Aiming)
            SwitchState(_factory.Empty());
    }

    public void FireBolt()
    {
        if (_timedOut)
        {
            _ctx.RangedAttackCooldown = _ctx.StartCoroutine(Cooldown());
            return;
        }
        _ctx.Animator.SetTrigger(_ctx.RangeFireTriggerHash);
        _timedOut = true; 
        AddBolt();
    }

    public void AddBolt()
    {
        var rotation = new Vector3(_ctx.MainCamera.rotation.eulerAngles.x, _ctx.transform.rotation.eulerAngles.y,0);
        //Debug.Log(rotation);
        //Debug.Log(Quaternion.identity);
        var obj = Object.Instantiate(_ctx.BoltPrefab, _ctx.BoltTransform.position,Quaternion.Euler(rotation));
    }

    private IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1f);
        _timedOut = false;
        _ctx.StopCoroutine(_ctx.RangedAttackCooldown);
    }
}