using UnityEngine;

internal sealed class PlayerAnimationController
{
    private static readonly int GroundedParameter = Animator.StringToHash("onfloor");
    private static readonly int MovementParameter = Animator.StringToHash("movement");
    private static readonly int AttackParameter = Animator.StringToHash("atack");
    private static readonly int DamageParameter = Animator.StringToHash("damage");
    private static readonly int DeathParameter = Animator.StringToHash("death");

    private readonly Animator animator;
    private readonly Transform playerTransform;

    internal PlayerAnimationController(Animator animator, Transform playerTransform)
    {
        this.animator = animator;
        this.playerTransform = playerTransform;
    }

    internal void SetGrounded(bool isGrounded)
    {
        animator.SetBool(GroundedParameter, isGrounded);
    }

    internal void SetMovement(float horizontalSpeed)
    {
        animator.SetFloat(MovementParameter, horizontalSpeed);
    }

    internal void FaceMovement(float horizontalInput)
    {
        if (horizontalInput < 0f)
            playerTransform.localScale = new Vector3(-1f, 1f, 1f);
        else if (horizontalInput > 0f)
            playerTransform.localScale = new Vector3(1f, 1f, 1f);
    }

    internal void SetAttacking(bool isAttacking)
    {
        animator.SetBool(AttackParameter, isAttacking);
    }

    internal void SetDamaged(bool isDamaged)
    {
        animator.SetBool(DamageParameter, isDamaged);
    }

    internal void SetDead()
    {
        animator.SetBool(DeathParameter, true);
    }
}
