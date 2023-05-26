using UnityEngine;

public class EnemyAttack : EnemyBaseState
{
    private int _attackTimer = 1;
    private float _coolDownTimer;
    private float _timer;
    public EnemyAttack(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory)
    {
    }

    public override void EnterState()
    {
        _timer = 0f;
        _isActive = true;
    }

    public override void UpdateState() //attack after the timer is reached and initiate a cool down.
    {
        if (_ctx.Alert && Vector3.Distance(_ctx.transform.position, _ctx.FOV.PlayerPosition) <= 1.5f &&
            _coolDownTimer == 0)
        {
            _timer += 1f * Time.deltaTime;
        }
        if (_timer > _attackTimer)
        {
            _ctx.Animator.SetTrigger(_ctx.SwingHash);
            _timer = 0;
            _coolDownTimer = 1;
        }

        if (_coolDownTimer > 0)
            _coolDownTimer -= Time.deltaTime;

        _coolDownTimer = Mathf.Clamp(_coolDownTimer, 0, 1);
        _timer = Mathf.Clamp(_timer, 0, 1);
    }

    public override void ExitState()
    {
        _isActive = false;
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchState()
    {
    }
}
