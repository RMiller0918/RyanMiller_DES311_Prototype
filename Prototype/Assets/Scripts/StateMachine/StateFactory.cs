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
    grounded,
    falling,
    stunned,
    unsuspicious,
    suspicious,
    alerted,
    idle,
    walk,
    run,
    attack,
}

public class EnemyStateFactory
{
    EnemyStateMachine _context;
    Dictionary<EnemyStates, EnemyBaseState> _states = new Dictionary<EnemyStates, EnemyBaseState>();

    public EnemyStateFactory(EnemyStateMachine currentContext)
    {
        _context = currentContext;
        _states[EnemyStates.grounded] = new EnemyGrounded(_context, this);
        _states[EnemyStates.falling] = new EnemyFalling(_context, this);
        _states[EnemyStates.stunned] = new EnemyStunned(_context, this);
        _states[EnemyStates.unsuspicious] = new EnemyUnsuspicious(_context, this);
        _states[EnemyStates.suspicious] = new EnemySuspicious(_context, this);
        _states[EnemyStates.alerted] = new EnemyAlerted(_context, this);
        _states[EnemyStates.idle] = new EnemyIdle(_context, this);
        _states[EnemyStates.walk] = new EnemyWalk(_context, this);
        _states[EnemyStates.run] = new EnemyRun(_context, this);
        _states[EnemyStates.attack] = new EnemyAttack(_context, this);
    }

    public EnemyBaseState Grounded()
    {
        return _states[EnemyStates.grounded];
    }

    public EnemyBaseState Falling()
    {
        return _states[EnemyStates.falling];
    }

    public EnemyBaseState Stunned()
    {
        return _states[EnemyStates.stunned];
    }

    public EnemyBaseState Unsuspicious()
    {
        return _states[EnemyStates.unsuspicious];
    }

    public EnemyBaseState Suspicious()
    {
        return _states[EnemyStates.suspicious];
    }

    public EnemyBaseState Alerted()
    {
        return _states[EnemyStates.alerted];
    }

    public EnemyBaseState Idle()
    {
        return _states[EnemyStates.idle];
    }

    public EnemyBaseState Walk()
    {
        return _states[EnemyStates.walk];
    }

    public EnemyBaseState Run()
    {
        return _states[EnemyStates.run];
    }

    public EnemyBaseState Attack()
    {
        return _states[EnemyStates.attack];
    }
}
