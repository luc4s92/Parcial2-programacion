using UnityEngine;

internal sealed class EnemyTargeting
{
    private readonly Transform owner;
    private readonly Transform target;
    private readonly float detectionRange;
    private readonly float attackRange;
    private bool targetIsAlive;

    internal bool HasTarget => target != null && targetIsAlive;
    internal bool IsTargetDetected => HasTarget && DistanceToTarget <= detectionRange;
    internal bool IsTargetInAttackRange => HasTarget && DistanceToTarget <= attackRange;
    internal Transform Target => target;

    private float DistanceToTarget => Vector2.Distance(owner.position, target.position);

    internal EnemyTargeting(
        Transform owner,
        Transform target,
        float detectionRange,
        float attackRange)
    {
        this.owner = owner;
        this.target = target;
        this.detectionRange = detectionRange;
        this.attackRange = attackRange;
        targetIsAlive = target != null;
    }

    internal void DisableTarget()
    {
        targetIsAlive = false;
    }
}
