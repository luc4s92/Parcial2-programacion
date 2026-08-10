using UnityEngine;

internal sealed class PlayerAnimationController
{
    private static readonly int GroundedParameter = Animator.StringToHash("onfloor");
    private static readonly int MovementParameter = Animator.StringToHash("movement");
    private static readonly int AttackParameter = Animator.StringToHash("atack");
    private static readonly int DamageParameter = Animator.StringToHash("damage");
    private static readonly int DeathParameter = Animator.StringToHash("death");
    private static readonly int AttackState = Animator.StringToHash("Base Layer.atack");
    private static readonly int RangedAttackState = Animator.StringToHash("Base Layer.throw");
    private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
    private static readonly int RunState = Animator.StringToHash("Base Layer.run");
    private static readonly int JumpState = Animator.StringToHash("Base Layer.jump");
    private static readonly int FallState = Animator.StringToHash("Base Layer.fall");

    private readonly Animator animator;
    private readonly Transform playerTransform;
    private bool isActionAnimationPlaying;
    private float horizontalSpeed;

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
        this.horizontalSpeed = horizontalSpeed;
        animator.SetFloat(MovementParameter, horizontalSpeed);
    }

    internal void PlayJump()
    {
        if (isActionAnimationPlaying) return;

        animator.Play(JumpState, 0, 0f);
    }

    internal void PlayFall()
    {
        if (isActionAnimationPlaying) return;

        animator.Play(FallState, 0, 0f);
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
        isActionAnimationPlaying = isAttacking;
        animator.SetBool(AttackParameter, isAttacking);

        if (isAttacking)
            animator.Play(AttackState, 0, 0f);
    }

    internal void PlayRangedAttack()
    {
        isActionAnimationPlaying = true;
        animator.Play(RangedAttackState, 0, 0f);
    }

    internal void StopRangedAttack()
    {
        isActionAnimationPlaying = false;
    }

    internal void PlayGrounded()
    {
        int state = Mathf.Abs(horizontalSpeed) > 0.01f
            ? RunState
            : IdleState;
        animator.Play(state, 0, 0f);
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
