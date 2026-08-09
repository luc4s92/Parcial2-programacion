using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public sealed class EnemyProjectile : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer;
    private Transform owner;
    private Action<EnemyProjectile> releaseToPool;
    private int damage;
    private float remainingLifetime;
    private bool isInUse;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        remainingLifetime -= Time.deltaTime;
        if (remainingLifetime <= 0f)
            Release();
    }

    internal void Initialize(
        Transform projectileOwner,
        Vector3 spawnPosition,
        Vector2 direction,
        float speed,
        int projectileDamage,
        float lifetime,
        Action<EnemyProjectile> releaseAction)
    {
        owner = projectileOwner;
        releaseToPool = releaseAction;
        damage = projectileDamage;
        remainingLifetime = lifetime;
        isInUse = true;

        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);

        if (spriteRenderer != null)
            spriteRenderer.flipX = direction.x < 0f;

        gameObject.SetActive(true);
        rigidBody.linearVelocity = direction.normalized * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (BelongsToOwner(collision.transform)) return;
        if (collision.GetComponentInParent<EnemyController>() != null) return;

        IDamageable damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable != null && damageable.IsAlive)
        {
            Vector2 direction = rigidBody.linearVelocity.normalized;
            damageable.TakeDamage(damage, direction);
            Release();
            return;
        }

        if (!collision.isTrigger)
            Release();
    }

    private bool BelongsToOwner(Transform other)
    {
        return owner != null && (other == owner || other.IsChildOf(owner));
    }

    private void Release()
    {
        if (!isInUse) return;

        isInUse = false;
        rigidBody.linearVelocity = Vector2.zero;
        owner = null;
        damage = 0;
        remainingLifetime = 0f;

        Action<EnemyProjectile> releaseAction = releaseToPool;
        releaseToPool = null;
        gameObject.SetActive(false);
        releaseAction?.Invoke(this);
    }
}
