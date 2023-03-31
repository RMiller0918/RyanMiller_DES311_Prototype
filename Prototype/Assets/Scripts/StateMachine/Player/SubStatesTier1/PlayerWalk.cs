using UnityEngine;

public class PlayerWalk : PlayerBaseState, IMoveable
{
    private int _moveSpeed;

    public PlayerWalk(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _moveSpeed = 4;
    }

    public override void EnterState()
    {
        Debug.Log("Walk State");
        _isSwitchingState = false;
    }

    public override void UpdateState()
    {
        CheckSwitchState();
        if (_isSwitchingState)
        {
            Debug.Log("Blocking Walking after Switch. Switching State: " + _isSwitchingState);
            return;
        }
        Moving();
    }

    public override void ExitState()
    {
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchState()
    {
        if (!_ctx.Moving)
        {
            SwitchState(_factory.Idle());
        }
        else if (_ctx.Sprinting)
        {
            SwitchState(_factory.Run());
        }
    }

    public void Moving()
    {
        var velocity = _ctx.transform.right * _ctx.MoveInput.x + _ctx.transform.forward * _ctx.MoveInput.y;
        _ctx.CharCont.Move(velocity * _moveSpeed * Time.deltaTime);
    }
}
