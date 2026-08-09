using UnityEngine;

public sealed class RangedEnemy : EnemyController
{
    [Header("Ranged combat")]
    [SerializeField] private EnemyProjectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 1.5f;
    [SerializeField] private float projectileSpeed = 6f;
    [SerializeField] private float projectileLifetime = 5f;
    [SerializeField] private int projectileDamage = 1;
    [SerializeField] private int projectilePoolCapacity = 8;

    private EnemyTargeting targeting;
    private EnemyProjectilePool projectilePool;
    private EnemyRangedCombat combat;
    private EnemyRangedIdleState idleState;
    private EnemyRangedAttackState attackState;

    private protected override IState CreateInitialState()
    {
        projectilePool = new EnemyProjectilePool(
            projectilePrefab,
            projectilePoolCapacity
        );
        targeting = new EnemyTargeting(
            transform,
            Player,
            detectionRadius,
            detectionRadius
        );
        combat = new EnemyRangedCombat(
            transform,
            firePoint,
            projectilePool,
            fireCooldown,
            projectileSpeed,
            projectileLifetime,
            projectileDamage
        );
        idleState = new EnemyRangedIdleState(
            targeting,
            Movement,
            AnimationController,
            ChangeToAttackState
        );
        attackState = new EnemyRangedAttackState(
            targeting,
            Movement,
            AnimationController,
            combat,
            ChangeToIdleState
        );

        return idleState;
    }

    protected override void OnDestroy()
    {
        projectilePool?.Dispose();
        base.OnDestroy();
    }

    protected override void TickBehaviour(float deltaTime)
    {
        combat.Tick(deltaTime);
    }

    protected override void HandleTargetDisabled()
    {
        targeting.DisableTarget();
    }

    protected override void ResolveBehaviourState()
    {
        if (targeting.IsTargetDetected)
            ChangeToAttackState();
        else
            ChangeToIdleState();
    }

    protected override void DropItem()
    {
    }

    private void ChangeToIdleState()
    {
        ChangeState(idleState);
    }

    private void ChangeToAttackState()
    {
        ChangeState(attackState);
    }
}
