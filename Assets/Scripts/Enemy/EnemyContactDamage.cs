using UnityEngine;

internal sealed class EnemyContactDamage
{
    private readonly Transform attacker;
    private readonly int damage;
    private readonly float cooldown;
    private float cooldownRemaining;

    internal EnemyContactDamage(Transform attacker, int damage, float cooldown)
    {
        this.attacker = attacker;
        this.damage = Mathf.Max(damage, 0);
        this.cooldown = Mathf.Max(cooldown, 0f);
    }

    internal void Tick(float deltaTime)
    {
        if (cooldownRemaining > 0f)
            cooldownRemaining -= deltaTime;
    }

    internal void TryApply(Collider2D collision)
    {
        if (cooldownRemaining > 0f) return;

        IDamageable damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable == null || !damageable.IsAlive) return;

        Vector2 direction = new Vector2(
            collision.transform.position.x - attacker.position.x,
            0.25f
        ).normalized;

        damageable.TakeDamage(damage, direction);
        cooldownRemaining = cooldown;
    }
}
