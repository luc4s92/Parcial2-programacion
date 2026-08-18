using UnityEngine;

internal sealed class EnemyTargeting
{
    private readonly Transform owner;
    private readonly Transform target;
    private readonly Transform sightOrigin;
    private readonly Collider2D targetCollider;
    private readonly float detectionRange;
    private readonly float attackRange;
    private readonly LayerMask sightObstructionLayers;
    private bool targetIsAlive;

    internal bool HasTarget => target != null && targetIsAlive;
    internal bool IsTargetDetected => IsTargetWithinRange(detectionRange);
    internal bool IsTargetInAttackRange => IsTargetWithinRange(attackRange);
    internal Transform Target => target;

    private float DistanceToTarget => Vector2.Distance(owner.position, target.position);

    internal EnemyTargeting(
        Transform owner,
        Transform target,
        Transform sightOrigin,
        float detectionRange,
        float attackRange,
        LayerMask sightObstructionLayers)
    {
        this.owner = owner;
        this.target = target;
        this.sightOrigin = sightOrigin != null ? sightOrigin : owner;
        targetCollider = target != null ? target.GetComponent<Collider2D>() : null;
        this.detectionRange = detectionRange;
        this.attackRange = attackRange;
        this.sightObstructionLayers = sightObstructionLayers;
        targetIsAlive = target != null;
    }

    internal void DisableTarget()
    {
        targetIsAlive = false;
    }

    private bool IsTargetWithinRange(float range)
    {
        return HasTarget &&
               DistanceToTarget <= range &&
               HasLineOfSight();
    }

    private bool HasLineOfSight()
    {
        if (sightObstructionLayers.value == 0)
            return true;

        RaycastHit2D obstruction = Physics2D.Linecast(
            sightOrigin.position,
            GetTargetSightPosition(),
            sightObstructionLayers
        );

        return obstruction.collider == null;
    }

    private Vector2 GetTargetSightPosition()
    {
        return targetCollider != null
            ? targetCollider.bounds.center
            : target.position;
    }
}
