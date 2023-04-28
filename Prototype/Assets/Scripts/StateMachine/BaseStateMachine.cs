using UnityEngine;

public abstract class BaseState
{
    protected bool _isRootState = false;
    protected bool _isSwitchingState = false;

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchState();
    public abstract void InitializeSubState();
}
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
        if (_currentSubState != null)
        {
            _currentSubState.ExitStates();
            
        }
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

    protected void SetSubState(PlayerBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
        newSubState.EnterState();
    }
}

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
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    //exit the state and any substates
    public void ExitStates()
    {
        ExitState();
        if (_currentSubState != null)
        {
            _currentSubState.ExitStates();
        }
    }

    protected void SwitchState(EnemyBaseState newState)
    {
        //current state exits state
        ExitState();

        //new state enters state
        newState.EnterState();

        if (_isRootState)
        {
            _ctx.CurrentState = newState;

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

    protected void SetSubState(EnemyBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }

}
