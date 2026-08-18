internal sealed class PlayerAttackState : IState
{
    private readonly PlayerAnimationController animationController;
    private readonly PlayerAudio playerAudio;
    private readonly PlayerMeleeHitbox meleeHitbox;

    internal PlayerAttackState(
        PlayerAnimationController animationController,
        PlayerAudio playerAudio,
        PlayerMeleeHitbox meleeHitbox)
    {
        this.animationController = animationController;
        this.playerAudio = playerAudio;
        this.meleeHitbox = meleeHitbox;
    }

    void IState.Enter()
    {
        meleeHitbox?.BeginAttack();
        animationController.SetAttacking(true);
        playerAudio?.PlaySwing();
    }

    void IState.Tick()
    {
    }

    void IState.Exit()
    {
        meleeHitbox?.EndAttack();
        animationController.SetAttacking(false);
    }
}
