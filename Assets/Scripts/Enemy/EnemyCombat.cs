using UnityEngine;

internal sealed class EnemyCombat
{
    private readonly Transform attacker;
    private readonly Collider2D attackHitbox;
    private readonly int attackDamage;
    private readonly float attackCooldown;

    private float cooldownRemaining;
    private bool attackInProgress;
    private bool damageApplied;

    internal bool CanAttack => cooldownRemaining <= 0f;

    internal EnemyCombat(
        Transform attacker,
        Collider2D attackHitbox,
        int attackDamage,
        float attackCooldown)
    {
        this.attacker = attacker;
        this.attackHitbox = attackHitbox;
        this.attackDamage = attackDamage;
        this.attackCooldown = attackCooldown;
        CloseHitbox();
    }

    internal void Tick(float deltaTime)
    {
        if (cooldownRemaining > 0f)
            cooldownRemaining -= deltaTime;
    }

    internal void BeginAttack()
    {
        attackInProgress = true;
        damageApplied = false;
        CloseHitbox();
    }

    internal void OpenHitbox()
    {
        if (attackInProgress && attackHitbox != null)
            attackHitbox.enabled = true;
    }

    internal void CloseHitbox()
    {
        if (attackHitbox != null)
            attackHitbox.enabled = false;
    }

    internal void TryDealDamage(Collider2D collision)
    {
        if (!attackInProgress || damageApplied) return;

        IDamageable damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable == null || !damageable.IsAlive) return;

        Vector2 direction = (collision.transform.position - attacker.position).normalized;
        damageable.TakeDamage(attackDamage, direction);
        damageApplied = true;
        CloseHitbox();
    }

    internal void EndAttack()
    {
        if (!attackInProgress) return;

        attackInProgress = false;
        cooldownRemaining = attackCooldown;
        CloseHitbox();
    }
}
