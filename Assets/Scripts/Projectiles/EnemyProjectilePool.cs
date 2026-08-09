using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class EnemyProjectilePool : IDisposable
{
    private readonly EnemyProjectile projectilePrefab;
    private readonly Queue<EnemyProjectile> availableProjectiles = new();
    private readonly int capacity;

    private int createdCount;
    private bool isDisposed;

    internal EnemyProjectilePool(EnemyProjectile projectilePrefab, int capacity)
    {
        this.projectilePrefab = projectilePrefab;
        this.capacity = Mathf.Max(capacity, 1);
    }

    internal EnemyProjectile Get()
    {
        if (isDisposed || projectilePrefab == null)
            return null;

        while (availableProjectiles.Count > 0)
        {
            EnemyProjectile projectile = availableProjectiles.Dequeue();
            if (projectile != null)
                return projectile;

            createdCount--;
        }

        if (createdCount >= capacity)
            return null;

        EnemyProjectile newProjectile = UnityEngine.Object.Instantiate(projectilePrefab);
        newProjectile.gameObject.SetActive(false);
        createdCount++;
        return newProjectile;
    }

    internal void Release(EnemyProjectile projectile)
    {
        if (projectile == null) return;

        if (isDisposed)
        {
            UnityEngine.Object.Destroy(projectile.gameObject);
            return;
        }

        availableProjectiles.Enqueue(projectile);
    }

    public void Dispose()
    {
        if (isDisposed) return;

        isDisposed = true;

        while (availableProjectiles.Count > 0)
        {
            EnemyProjectile projectile = availableProjectiles.Dequeue();
            if (projectile != null)
                UnityEngine.Object.Destroy(projectile.gameObject);
        }
    }
}
