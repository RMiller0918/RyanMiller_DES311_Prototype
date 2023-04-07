using System.Collections.Generic;

internal enum PlayerStates
{
    idle,
    walk,
    run,
    grounded,
    jump,
    falling,
    attack,
    rangedattack,
    teleport,
    pullenemy,
    healing,
    empty
}

public class PlayerStateFactory
{
    PlayerStateMachine _context;
    Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new PlayerIdle(_context, this);
        _states[PlayerStates.walk] = new PlayerWalk(_context, this);
        _states[PlayerStates.run] = new PlayerRun(_context, this);
        _states[PlayerStates.grounded] = new PlayerGrounded(_context, this);
        _states[PlayerStates.jump] = new PlayerJump(_context, this);
        _states[PlayerStates.falling] = new PlayerFalling(_context, this);
        _states[PlayerStates.attack] = new PlayerAttack(_context, this);
        _states[PlayerStates.rangedattack] = new PlayerRangedAttack(_context, this);
        _states[PlayerStates.teleport] = new PlayerTeleport(_context, this);
        _states[PlayerStates.pullenemy] = new PlayerPullEnemy(_context, this);
        _states[PlayerStates.healing] = new PlayerHealing(_context, this);
        _states[PlayerStates.empty] = new PlayerDefault(_context, this);
    }

    public PlayerBaseState Idle()
    {
        return _states[PlayerStates.idle];
    }

    public PlayerBaseState Walk()
    {
        return _states[PlayerStates.walk];
    }

    public PlayerBaseState Run()
    {
        return _states[PlayerStates.run];
    }

    public PlayerBaseState Grounded()
    {
        return _states[PlayerStates.grounded];
    }

    public PlayerBaseState Jump()
    {
        return _states[PlayerStates.jump];
    }

    public PlayerBaseState Falling()
    {
        return _states[PlayerStates.falling];
    }

    public PlayerBaseState Attack()
    {
        return _states[PlayerStates.attack];
    }

    public PlayerBaseState RangedAttack()
    {
        return _states[PlayerStates.rangedattack];
    }

    public PlayerBaseState Teleport()
    {
        return _states[PlayerStates.teleport];
    }

    public PlayerBaseState PullEnemy()
    {
        return _states[PlayerStates.pullenemy];
    }

    public PlayerBaseState Healing()
    {
        return _states[PlayerStates.healing];
    }

    public PlayerBaseState Empty() //Only used to switch to the other tier 2 states.
    {
        return _states[PlayerStates.empty];
    }
}

internal enum EnemyStates
{
    stunned,
    unsuspicious,
    suspicious,
    alerted,
    idle,
    walk,
    run,
    attack,
    heal,
}

public class EnemyStateFactory
{
    EnemyStateMachine _context;
    Dictionary<EnemyStates, EnemyBaseState> _states = new Dictionary<EnemyStates, EnemyBaseState>();

    public EnemyStateFactory(EnemyStateMachine currentContext)
    {
        _context = currentContext;
    }
}
