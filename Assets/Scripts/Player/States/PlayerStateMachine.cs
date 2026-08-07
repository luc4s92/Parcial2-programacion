internal sealed class PlayerStateMachine
{
    private IPlayerState currentState;

    internal PlayerStateMachine(IPlayerState initialState)
    {
        currentState = initialState;
        currentState.Enter();
    }

    internal void ChangeState(IPlayerState nextState)
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

    internal bool IsInState(IPlayerState state)
    {
        return currentState == state;
    }
}
