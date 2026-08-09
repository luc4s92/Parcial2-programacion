internal sealed class StateMachine
{
    private IState currentState;

    internal StateMachine(IState initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    internal void ChangeState(IState nextState)
    {
        if (nextState == null || nextState == currentState) return;

        currentState.Exit();
        currentState = nextState;
        currentState.Enter();
    }

    internal void Tick()
    {
        currentState.Tick();
    }

    internal bool IsInState(IState state)
    {
        return currentState == state;
    }
}
