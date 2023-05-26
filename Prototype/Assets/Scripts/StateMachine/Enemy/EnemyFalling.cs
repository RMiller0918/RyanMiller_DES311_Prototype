using UnityEngine;

public class EnemyFalling : EnemyBaseState, IGravity
{
    private float _fallTime;
    public EnemyFalling(EnemyStateMachine currentContext, EnemyStateFactory enemyStateFactory)
        : base(currentContext, enemyStateFactory)
    {
        _isRootState = true;
    }

    public override void EnterState() //enable the character controller and disable the NavMeshAgent. this is needed to apply gravity to the character.
    {
        _isActive = true;
        _ctx.CharCont.enabled = true;
        _ctx.Agent.enabled = false;
        _fallTime = 0;
        SetUpJumpVariables(); //calculate gravity values.
    }

    public override void UpdateState() //apply gravity and check if the enemy can switch state.
    {
        _fallTime += Time.deltaTime;
        HandleGravity();
        CheckSwitchState();
    }

    public override void ExitState() //check if the enemy has been falling for longer than the set times and apply damage accordingly.
    {
        switch (_fallTime)
        {
            case > 2f and < 5f:
                _ctx.HandleDamage(100);
                break;
            case >= 5f:
                _ctx.HandleDamage(_ctx.MaxHealth);
                break;
        }

        _isActive = false;
    }

    public override void InitializeSubState() //initialize suspicion States
    {
        switch (_ctx.Alertness)
        {
            case < 0.5f:
                SetSubState(_factory.Unsuspicious());
                break;
            case >= 0.5f:
                SetSubState(_factory.Suspicious());
                break;
            default:
            {
                if (_ctx.Alert)
                    SetSubState(_factory.Alerted());
                break;
            }
        }
    }

    public override void CheckSwitchState() //switch to grounded state
    {
        if(_ctx.IsGrounded)
            SwitchState(_factory.Grounded());
    }

    public void HandleGravity() //apply gravity increase the velocity each frame. 
    {
        var previousYVelocity = _ctx.MoveVelocityY;
        _ctx.MoveVelocityY += (_ctx.Gravity + Time.deltaTime);
        _ctx.AppliedMoveVelocityY = Mathf.Max((previousYVelocity + _ctx.MoveVelocityY) * .5f, -20.0f);
    }

    private void SetUpJumpVariables() //calculate gravity value.
    {
        var timeToApex = _ctx._maxJumpTime / 2;
        _ctx.Gravity = (-2 * _ctx._maxJumpHeight) / Mathf.Pow(timeToApex, 2);
    }

}
