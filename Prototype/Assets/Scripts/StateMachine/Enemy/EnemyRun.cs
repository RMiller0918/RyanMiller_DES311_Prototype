using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRun : EnemyBaseState
{
    //not used. 
    public EnemyRun(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory)
    {
    }

    public override void EnterState() { }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override void InitializeSubState() { }

    public override void CheckSwitchState() { }
}
