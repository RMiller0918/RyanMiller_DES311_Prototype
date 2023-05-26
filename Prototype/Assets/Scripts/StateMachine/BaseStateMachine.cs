//Abstract base class for the all states.
public abstract class BaseState
{
    protected bool _isRootState = false;
    protected bool _isSwitchingState = false;
    protected bool _isActive = false;
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchState();
    public abstract void InitializeSubState();
}

//The base state for the player states.
public abstract class PlayerBaseState: BaseState
{
    protected PlayerStateMachine _ctx;
    protected PlayerStateFactory _factory;
    protected PlayerBaseState _currentSubState;
    protected PlayerBaseState _currentSuperState;

    public PlayerBaseState(PlayerStateMachine currentContext, PlayerStateFactory stateFactory)
    {
        _ctx = currentContext;
        _factory = stateFactory;
    }

    //update the state and update any substates
    public void UpdateStates()
    {
        UpdateState();
        _currentSubState?.UpdateStates();
    }

    //exit the state and any substates
    public void ExitStates()
    {
        ExitState();
        _currentSubState?.ExitStates();
    }

    protected void SwitchState(PlayerBaseState newState)
    {
        //current state exits state
        ExitState();



        if (_isRootState)
        {
            _ctx.CurrentState = newState;
            //new state enters state
            newState.EnterState();
        }
        else if (_currentSuperState != null)
        {
            //the new state becomes a substate to the above layer in hierarchy.
            _currentSuperState.SetSubState(newState);
        }
    }


    protected void SetSuperState(PlayerBaseState newSuperState)
    {
        _currentSuperState = newSuperState;

    }

    protected void SetSubState(PlayerBaseState newSubState) //after setting a substate, enter the substate. Needed to allow for all substates to be updated. only running UpdateStates means only the first 2 layers are updated correctly.
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        if(!_currentSubState._isActive) //isActive needed to prevent an issue where nothing would happen when the player tries to use ability states.
            newSubState.EnterState();
    }
}

//Abstract class for the Enemy States.
public abstract class EnemyBaseState : BaseState 
{
    protected EnemyStateMachine _ctx;
    protected EnemyStateFactory _factory;
    protected EnemyBaseState _currentSubState;
    protected EnemyBaseState _currentSuperState;

    public EnemyBaseState(EnemyStateMachine currentContext, EnemyStateFactory stateFactory)
    {
        _ctx = currentContext;
        _factory = stateFactory;
    }

    //update the state and update any substates
    public void UpdateStates()
    {
        UpdateState();
        _currentSubState?.UpdateStates();
    }

    //exit the state and any substates
    public void ExitStates()
    {
        ExitState();
        _currentSubState?.ExitStates();
    }

    protected void SwitchState(EnemyBaseState newState)
    {
        //current state exits state
        ExitState();


        if (_isRootState)
        {
            _ctx.CurrentState = newState;
            //new state enters state
            newState.EnterState();
        }
        else if (_currentSuperState != null)
        {
            //Set the current super states Sub state to the new state
            _currentSuperState.SetSubState(newState);
        }
    }


    protected void SetSuperState(EnemyBaseState newSuperState)
    {
        _currentSuperState = newSuperState;

    }

    protected void SetSubState(EnemyBaseState newSubState) //Allows for More than 2 layers of states to be used at a time.
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        if (!_currentSubState._isActive)
            newSubState.EnterState();
    }

}
