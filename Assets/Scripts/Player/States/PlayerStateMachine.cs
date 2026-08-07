public sealed class PlayerStateMachine
{
    public IPlayerState CurrentState { get; private set; }

    public PlayerStateMachine(IPlayerState initialState)
    {
        CurrentState = initialState;
        CurrentState.Enter();
    }

    public void ChangeState(IPlayerState nextState)
    {
        if (nextState == null || nextState == CurrentState) return;

        CurrentState.Exit();
        CurrentState = nextState;
        CurrentState.Enter();
    }

    public void Tick()
    {
        CurrentState.Tick();
    }

    public bool IsInState(IPlayerState state)
    {
        return CurrentState == state;
    }
}
