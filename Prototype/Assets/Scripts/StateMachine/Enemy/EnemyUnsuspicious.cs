using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnsuspicious : EnemyBaseState
{
    public EnemyUnsuspicious(EnemyStateMachine currentCotext, EnemyStateFactory enemyStateFactory)
        : base(currentCotext, enemyStateFactory)
    {

    }

    public override void EnterState() { }

    public override void UpdateState() { }

    public override void ExitState() { }

    public override void InitializeSubState() { }

    public override void CheckSwitchState() { }
}