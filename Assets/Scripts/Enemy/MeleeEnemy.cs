using System.Collections.Generic;
using UnityEngine;

public sealed class MeleeEnemy : EnemyController
{
    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 0.75f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private Collider2D attackHitbox;

    [Header("Drops")]
    [SerializeField] private List<GameObject> possibleDrops;
    [Range(0f, 1f)]
    [SerializeField] private float dropChance = 1f;

    private EnemyTargeting targeting;
    private EnemyCombat combat;
    private EnemyIdleState idleState;
    private EnemyChaseState chaseState;
    private EnemyAttackState attackState;

    private protected override IState CreateInitialState()
    {
        targeting = new EnemyTargeting(transform, Player, detectionRadius, attackRange);
        combat = new EnemyCombat(transform, attackHitbox, attackDamage, attackCooldown);

        idleState = new EnemyIdleState(
            targeting,
            Movement,
            AnimationController,
            ChangeToChaseState
        );
        chaseState = new EnemyChaseState(
            targeting,
            Movement,
            AnimationController,
            combat,
            ChangeToIdleState,
            ChangeToAttackState
        );
        attackState = new EnemyAttackState(
            targeting,
            Movement,
            AnimationController,
            combat,
            ResolveBehaviourState
        );

        return idleState;
    }

    protected override void TickBehaviour(float deltaTime)
    {
        combat.Tick(deltaTime);
    }

    protected override void HandleTriggerEnter(Collider2D collision)
    {
        if (IsInState(attackState))
            combat.TryDealDamage(collision);
    }

    protected override void HandleTargetDisabled()
    {
        targeting.DisableTarget();
    }

    protected override void ResolveBehaviourState()
    {
        if (targeting.IsTargetDetected)
            ChangeToChaseState();
        else
            ChangeToIdleState();
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    public void BeginAttackHit()
    {
        if (IsInState(attackState))
            combat.OpenHitbox();
    }

    public void EndAttackHit()
    {
        combat.CloseHitbox();
    }

    public void EndAttack()
    {
        if (IsInState(attackState))
            attackState.CompleteAttack();
    }

    protected override void DropItem()
    {
        if (possibleDrops == null || possibleDrops.Count == 0) return;
        if (Random.value > dropChance) return;

        int index = Random.Range(0, possibleDrops.Count);
        GameObject itemToDrop = possibleDrops[index];
        Instantiate(itemToDrop, transform.position, Quaternion.identity);

        Debug.Log($"[{gameObject.name}] solto: {itemToDrop.name}");
    }

    private void ChangeToIdleState()
    {
        ChangeState(idleState);
    }

    private void ChangeToChaseState()
    {
        ChangeState(chaseState);
    }

    private void ChangeToAttackState()
    {
        ChangeState(attackState);
    }
}
