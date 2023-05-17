using UnityEngine;

public class PlayerDefault : PlayerBaseState
{
    public PlayerDefault(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        _isSwitchingState = false;
    }

    public override void EnterState()
    {
        _isActive = true;
        Debug.Log("Empty State. Current Super State =  " + _currentSuperState);
        _ctx.TeleMarker.ResetPosition();
        _ctx.TeleMarker.SetColor(Color.white);
        _ctx.Animator.SetBool(_ctx.TeleportSetupHash, false);
        _ctx.Animator.SetBool(_ctx.RangeSetUpHash, false);
        _isSwitchingState = false;
    }

    public override void UpdateState()
    {
        if (_isSwitchingState) return;
        CheckSwitchState();
    }

    public override void ExitState()
    {
        _isSwitchingState = false;
        _isActive = false;
    }

    public override void InitializeSubState()
    {
    }

    public override void CheckSwitchState()
    {
        if (_ctx.Attacking && !_ctx.NewAttackRequired && !_ctx.TeleportSetUp && !_ctx.PullEnemySetUp && !_ctx.Aiming)
        {
            if (_ctx.NewAttackRequired) return;
            SwitchState(_factory.Attack());
            _isSwitchingState = true;
        }
        else if (_ctx.Aiming && _ctx.Mana > 15f)
        {
            SwitchState(_factory.RangedAttack());
            _isSwitchingState = true;
        }
        else if (_ctx.TeleportSetUp && !_ctx.NewTeleSetUpRequired && !_ctx.Lit && _ctx.Mana > 25f)
        {
            Debug.Log($"Forcing into Teleport State, even with newTeleSetUp = {_ctx.NewTeleSetUpRequired}");
            if (_ctx.NewTeleSetUpRequired) return;
            Debug.Log($"Ive made it past the return, even with newTeleSetUp = {_ctx.NewTeleSetUpRequired}");
            SwitchState(_factory.Teleport());
            _isSwitchingState = true;
        }
        else if (_ctx.PullEnemySetUp && !_ctx.NewPullRequired && !_ctx.Lit && _ctx.Mana > 40f)
        {
            if (_ctx.NewPullRequired) return;
            SwitchState(_factory.PullEnemy());
            _isSwitchingState = true;
        }
        else if (_ctx.Healing && !_ctx.NewHealRequired && _ctx.Mana > 65f)
        {
            SwitchState(_factory.Healing());
            _isSwitchingState = true;
        }
    }
}
