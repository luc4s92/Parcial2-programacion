using System;
using UnityEngine;

internal sealed class EnemyRangedCombat
{
    private readonly Transform owner;
    private readonly Transform firePoint;
    private readonly ComponentPool<EnemyProjectile> projectilePool;
    private readonly Action<EnemyProjectile> releaseProjectile;
    private readonly float fireCooldown;
    private readonly float projectileSpeed;
    private readonly float projectileLifetime;
    private readonly int projectileDamage;
    private float cooldownRemaining;

    internal bool CanFire => cooldownRemaining <= 0f;

    internal EnemyRangedCombat(
        Transform owner,
        Transform firePoint,
        ComponentPool<EnemyProjectile> projectilePool,
        float fireCooldown,
        float projectileSpeed,
        float projectileLifetime,
        int projectileDamage)
    {
        this.owner = owner;
        this.firePoint = firePoint != null ? firePoint : owner;
        this.projectilePool = projectilePool;
        releaseProjectile = projectilePool != null
            ? projectilePool.Release
            : null;
        this.fireCooldown = Mathf.Max(fireCooldown, 0.05f);
        this.projectileSpeed = Mathf.Max(projectileSpeed, 0.1f);
        this.projectileLifetime = Mathf.Max(projectileLifetime, 0.1f);
        this.projectileDamage = Mathf.Max(projectileDamage, 0);
    }

    internal void Tick(float deltaTime)
    {
        if (cooldownRemaining > 0f)
            cooldownRemaining -= deltaTime;
    }

    internal bool TryFire(Transform target)
    {
        if (cooldownRemaining > 0f || projectilePool == null || target == null)
            return false;

        Vector2 direction = target.position - firePoint.position;
        if (direction.sqrMagnitude <= 0.001f)
            direction = owner.localScale.x < 0f ? Vector2.right : Vector2.left;

        EnemyProjectile projectile = projectilePool.Get();
        if (projectile == null)
            return false;

        projectile.Initialize(
            owner,
            firePoint.position,
            direction,
            projectileSpeed,
            projectileDamage,
            projectileLifetime,
            releaseProjectile
        );

        cooldownRemaining = fireCooldown;
        return true;
    }
}
