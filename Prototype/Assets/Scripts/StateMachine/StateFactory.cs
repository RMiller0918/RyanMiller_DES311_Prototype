public class PlayerStateFactory
{
    PlayerStateMachine _context;

    public PlayerStateFactory(PlayerStateMachine currentContext)
    {
        _context = currentContext;
    }

    public PlayerBaseState Idle()
    {
        return new PlayerIdle(_context, this);
    }

    public PlayerBaseState Walk()
    {
        return new PlayerWalk(_context, this);
    }

    public PlayerBaseState Run()
    {
        return new PlayerRun(_context, this);
    }

    public PlayerBaseState Jump()
    {
        return new PlayerJump(_context, this);
    }

    public PlayerBaseState Teleport()
    {
        return new PlayerTeleport(_context, this);
    }
}

public class EnemyStateFactory
{
    EnemyStateMachine _context;
}
