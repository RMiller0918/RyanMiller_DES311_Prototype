using System.Collections.Generic;

enum PlayerStates
{
    idle,
    walk,
    run,
    grounded,
    jump,
    teleport,
}
public class PlayerStateFactory
{
    PlayerStateMachine _context;
    private Dictionary<PlayerStates, PlayerBaseState> _states = new Dictionary<PlayerStates, PlayerBaseState>();

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStates.idle] = new PlayerIdle(_context, this);
        _states[PlayerStates.walk] = new PlayerWalk(_context, this);
        _states[PlayerStates.run] = new PlayerRun(_context, this);
        _states[PlayerStates.jump] = new PlayerJump(_context, this);
        _states[PlayerStates.teleport] = new PlayerTeleport(_context, this);
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

    public PlayerBaseState Teleport()
    {
        return _states[PlayerStates.teleport];
    }
}

public class EnemyStateFactory
{
    EnemyStateMachine _context;
}
