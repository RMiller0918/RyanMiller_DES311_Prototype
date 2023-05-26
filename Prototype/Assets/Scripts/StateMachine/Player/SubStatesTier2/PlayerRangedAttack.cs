
using UnityEngine;

public class PlayerRangedAttack : PlayerBaseState
{
    private int hitCount;
    public PlayerRangedAttack(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState() //Play initial animation and change the Range UI Icon.
    {
        _ctx.RangeIcon.SpriteChange(1);
        _isActive = true;
        _ctx.Animator.SetBool(_ctx.RangeSetUpHash, true);
    }

    public override void UpdateState()
    {
        CheckSwitchState(); //check if the switch conditions are met
        if(_ctx.Attacking && !_ctx.NewAttackRequired)
            StartAnimation();
    }

    public override void ExitState() //Reset the UI Icon and return to the idle animation state.
    {
        _ctx.RangeIcon.SpriteChange(0);
        _ctx.OrbScript.TriggerStart(false);
        _ctx.Animator.SetBool(_ctx.RangeSetUpHash, false);
        _isActive = false;
        hitCount = 0;
        if (_ctx.Attacking) //block spamming a melee attack upon exit if the player is still holding Left Click
            _ctx.NewAttackRequired = true;
    }

    public override void InitializeSubState() //no other tier of states
    {
    }

    public override void CheckSwitchState() //if the player releases Right-click or they don't have enough mana, return the player to the empty state.
    {
        if (!_ctx.Aiming || _ctx.Mana <= 15f)
            SwitchState(_factory.Empty());
    }

    private void StartAnimation() //Starts animation if the player left-clicks. Prevented from clicking again if the player holds the button.
    {
        if (_ctx.Attacking)
            _ctx.NewAttackRequired = true;
        _ctx.Animator.SetTrigger(_ctx.RangeFireTriggerHash);
    }
}