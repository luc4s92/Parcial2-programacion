using UnityEngine;
using System.Collections.Generic;

internal sealed class EnemyAnimationController
{
    private static readonly int MovementParameter = Animator.StringToHash("onMovement");
    private static readonly int DamageParameter = Animator.StringToHash("damage");
    private static readonly int DeathParameter = Animator.StringToHash("death");
    private static readonly int AttackParameter = Animator.StringToHash("isAttacking");
    private static readonly int IdleState = Animator.StringToHash("Base Layer.idle");
    private static readonly int RunState = Animator.StringToHash("Base Layer.run");

    private readonly Animator animator;
    private readonly HashSet<int> boolParameters = new HashSet<int>();

    internal EnemyAnimationController(Animator animator)
    {
        this.animator = animator;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.type == AnimatorControllerParameterType.Bool)
                boolParameters.Add(parameter.nameHash);
        }
    }

    internal void SetMoving(bool isMoving)
    {
        if (boolParameters.Contains(MovementParameter))
            animator.SetBool(MovementParameter, isMoving);
        else
            animator.Play(isMoving ? RunState : IdleState);
    }

    internal void SetDamaged(bool isDamaged)
    {
        SetBool(DamageParameter, isDamaged);
    }

    internal void SetAttacking(bool isAttacking)
    {
        SetBool(AttackParameter, isAttacking);
    }

    internal void SetDead()
    {
        SetBool(DeathParameter, true);
    }

    private void SetBool(int parameter, bool value)
    {
        if (boolParameters.Contains(parameter))
            animator.SetBool(parameter, value);
    }
}
