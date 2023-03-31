using UnityEngine;

public class PlayerRun : PlayerBaseState, IMoveable
{
    private int _moveSpeed;

    public PlayerRun(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _moveSpeed = 7;
    }

    public override void EnterState()
    {
        Debug.Log("Run State");
        _isSwitchingState = false;
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if (_isSwitchingState)
        {
            Debug.Log("Blocking Running after Switch. Switching State: " + _isSwitchingState);
            return;
        }
        Moving();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchState()
    {
        if (!_ctx.Sprinting && _ctx.Moving)
        {
            SwitchState(_factory.Walk());
            _isSwitchingState = true;
        }
        else if (!_ctx.Moving)
        {
            SwitchState(_factory.Idle());
        }
    }

    public void Moving()
    {
        if(_isSwitchingState)
            Debug.Log("Moving After Switching");
        var velocity = _ctx.transform.right * _ctx.MoveInput.x + _ctx.transform.forward * _ctx.MoveInput.y;
        _ctx.CharCont.Move(velocity * _moveSpeed * Time.deltaTime);
    }
}
