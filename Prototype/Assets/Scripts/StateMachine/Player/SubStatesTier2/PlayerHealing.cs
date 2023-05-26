using UnityEngine;


/// <summary>
/// This state will heal the player only if they have less than full health.
/// Will heal up to 75 max Health, but will exit early if the player has full health before adding the 75.
/// </summary>
public class PlayerHealing : PlayerBaseState
{
    private float _healthReturn = 75;
    private float _newMaxHealth;
    public PlayerHealing(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState() //Remove mana from the player and add Health to the player. 
    {
        _ctx.Animator.SetTrigger(_ctx.HealTriggerHash);
        _ctx.Mana -= 65f;
        _ctx.Mana = Mathf.Clamp(_ctx.Mana, 0, _ctx.MaxMana);
        _ctx.MBar.UpdateHealthBar(_ctx.MaxMana, _ctx.Mana);
        _healthReturn = 75f;
        _newMaxHealth = _ctx.Health + _healthReturn;
        _newMaxHealth = Mathf.Clamp(_newMaxHealth, 0, _ctx.MaxHealth);
        _isActive = true;
    }

    public override void UpdateState()
    {
        if (_ctx.Healing) //update the health bar
        {
            _ctx.Health = Mathf.Lerp(_ctx.Health, _newMaxHealth, 5f * Time.deltaTime);
            _ctx.Health = Mathf.Clamp(_ctx.Health, 0, _ctx.MaxHealth);
            _ctx.HBar.UpdateHealthBar(_ctx.MaxHealth, _ctx.Health);
        }
        else if (!_ctx.Healing || _ctx.Health >= _newMaxHealth) //switch back to the empty state.
        {
            CheckSwitchState();
        }
    }

    public override void ExitState()
    {
        _isActive = false;
        if (_ctx.Healing)
            _ctx.NewHealRequired = true;
    }

    public override void InitializeSubState()
    {

    }

    public override void CheckSwitchState()
    {
        SwitchState(_factory.Empty());
    }
}