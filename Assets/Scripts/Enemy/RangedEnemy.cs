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
    [SerializeField, Min(0f)] private float fireWindupDuration = 0.45f;
    [SerializeField, Min(0f)] private float fireRecoveryDuration = 0.25f;
    [SerializeField] private Color fireTelegraphColor = new(1f, 0.35f, 0.15f, 1f);
    [SerializeField, Min(0.1f)] private float fireTelegraphPulseSpeed = 6f;

    private EnemyTargeting targeting;
    private ComponentPool<EnemyProjectile> projectilePool;
    private EnemyRangedCombat combat;
    private EnemyRangedIdleState idleState;
    private EnemyRangedAttackState attackState;
    private EnemyAttackWindupState windupState;
    private EnemyRangedFireState fireState;

    private protected override IState CreateInitialState()
    {
        projectilePool = new ComponentPool<EnemyProjectile>(
            projectilePrefab,
            projectilePoolCapacity
        );
        targeting = new EnemyTargeting(
            transform,
            Player,
            SightOrigin,
            detectionRadius,
            detectionRadius,
            SightObstructionLayers
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
        EnemyAttackTelegraph telegraph = new EnemyAttackTelegraph(
            GetComponent<SpriteRenderer>(),
            fireTelegraphColor,
            fireTelegraphPulseSpeed
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
            ChangeToIdleState,
            ChangeToWindupState
        );
        windupState = new EnemyAttackWindupState(
            targeting,
            Movement,
            telegraph,
            fireWindupDuration,
            () => targeting.IsTargetDetected,
            ChangeToIdleState,
            ChangeToFireState
        );
        fireState = new EnemyRangedFireState(
            targeting,
            Movement,
            combat,
            fireRecoveryDuration,
            ResolveBehaviourState
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

    private void ChangeToWindupState()
    {
        ChangeState(windupState);
    }

    private void ChangeToFireState()
    {
        ChangeState(fireState);
    }
}
