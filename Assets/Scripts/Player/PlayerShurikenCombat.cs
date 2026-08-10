using System;
using UnityEngine;

internal sealed class PlayerShurikenCombat
{
    private readonly Transform owner;
    private readonly Transform firePoint;
    private readonly ComponentPool<PlayerShurikenProjectile> projectilePool;
    private readonly Action<PlayerShurikenProjectile> releaseProjectile;
    private readonly float cooldown;
    private readonly float projectileSpeed;
    private readonly float projectileRotationSpeed;
    private readonly float projectileLifetime;
    private readonly int projectileDamage;

    private float cooldownRemaining;

    internal PlayerShurikenCombat(
        Transform owner,
        Transform firePoint,
        ComponentPool<PlayerShurikenProjectile> projectilePool,
        float cooldown,
        float projectileSpeed,
        float projectileRotationSpeed,
        float projectileLifetime,
        int projectileDamage)
    {
        this.owner = owner;
        this.firePoint = firePoint != null ? firePoint : owner;
        this.projectilePool = projectilePool;
        releaseProjectile = projectilePool != null
            ? projectilePool.Release
            : null;
        this.cooldown = Mathf.Max(cooldown, 0.05f);
        this.projectileSpeed = Mathf.Max(projectileSpeed, 0.1f);
        this.projectileRotationSpeed = Mathf.Abs(projectileRotationSpeed);
        this.projectileLifetime = Mathf.Max(projectileLifetime, 0.1f);
        this.projectileDamage = Mathf.Max(projectileDamage, 0);
    }

    internal bool CanThrow =>
        cooldownRemaining <= 0f &&
        projectilePool != null &&
        projectilePool.CanGet;

    internal void Tick(float deltaTime)
    {
        if (cooldownRemaining > 0f)
            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - deltaTime);
    }

    internal bool TryThrow()
    {
        if (!CanThrow)
            return false;

        PlayerShurikenProjectile projectile = projectilePool.Get();
        if (projectile == null)
            return false;

        Vector2 direction = owner.localScale.x < 0f
            ? Vector2.left
            : Vector2.right;

        projectile.Initialize(
            owner,
            firePoint.position,
            direction,
            projectileSpeed,
            projectileRotationSpeed,
            projectileDamage,
            projectileLifetime,
            releaseProjectile
        );

        cooldownRemaining = cooldown;
        return true;
    }
}
