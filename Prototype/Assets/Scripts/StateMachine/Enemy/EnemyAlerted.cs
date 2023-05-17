using UnityEngine;

public class EnemyAlerted : EnemyBaseState
{
    public EnemyAlerted(EnemyStateMachine currentCotext, EnemyStateFactory enemyStateFactory)
    :base(currentCotext, enemyStateFactory)
    {

    }

    public override void EnterState() {}

    public override void UpdateState() {}

    public override void ExitState() {}

    public override void InitializeSubState() {}

    public override void CheckSwitchState() { }

}
