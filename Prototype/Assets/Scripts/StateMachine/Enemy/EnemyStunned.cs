
using UnityEngine;

public class EnemyStunned : EnemyBaseState
{
    //not used
    private float _timer;
    public EnemyStunned(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory) { }

    public override void EnterState()
    {
        _timer = 0f;
        _ctx.Agent.SetDestination(_ctx.transform.position);
    }

    public override void UpdateState()
    {
        if (_ctx.Agent.isOnNavMesh)
            _ctx.Agent.isStopped = true;
        _timer += Time.deltaTime;
        if(_timer > 2f)
            CheckSwitchState();
    }

    public override void ExitState()
    {
        _ctx.Stunned = false;
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchState()
    {
        SwitchState(_factory.Idle());
    }
}
