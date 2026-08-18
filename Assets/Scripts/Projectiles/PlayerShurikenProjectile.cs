using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public sealed class PlayerShurikenProjectile : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    private Transform owner;
    private Action<PlayerShurikenProjectile> releaseToPool;
    private int damage;
    private float remainingLifetime;
    private bool isInUse;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isInUse) return;

        remainingLifetime -= Time.deltaTime;
        if (remainingLifetime <= 0f)
            Release();
    }

    internal void Initialize(
        Transform projectileOwner,
        Vector3 spawnPosition,
        Vector2 direction,
        float speed,
        float rotationSpeed,
        int projectileDamage,
        float lifetime,
        Action<PlayerShurikenProjectile> releaseAction)
    {
        owner = projectileOwner;
        releaseToPool = releaseAction;
        damage = Mathf.Max(projectileDamage, 0);
        remainingLifetime = Mathf.Max(lifetime, 0.1f);
        isInUse = true;

        Vector2 normalizedDirection = direction.sqrMagnitude > 0.001f
            ? direction.normalized
            : Vector2.right;

        transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        gameObject.SetActive(true);
        rigidBody.linearVelocity = normalizedDirection * Mathf.Max(speed, 0.1f);
        rigidBody.angularVelocity = -Mathf.Sign(normalizedDirection.x) * Mathf.Abs(rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isInUse || BelongsToOwner(collision.transform)) return;

        EnemyController enemy = collision.GetComponentInParent<EnemyController>();
        if (enemy != null)
        {
            if (enemy.IsAlive)
            {
                Vector2 sourcePosition = owner != null
                    ? owner.position
                    : transform.position;
                enemy.TryTakeDamage(sourcePosition, damage);
            }

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
        rigidBody.angularVelocity = 0f;
        owner = null;
        damage = 0;
        remainingLifetime = 0f;

        Action<PlayerShurikenProjectile> releaseAction = releaseToPool;
        releaseToPool = null;
        gameObject.SetActive(false);
        releaseAction?.Invoke(this);
    }
}
