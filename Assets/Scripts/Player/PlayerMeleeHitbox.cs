using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public sealed class PlayerMeleeHitbox : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 1;

    private readonly HashSet<EnemyController> damagedEnemies = new();
    private Collider2D hitboxCollider;
    private Transform owner;
    private Action playHitFeedback;
    private bool feedbackPlayed;

    private void Awake()
    {
        hitboxCollider = GetComponent<Collider2D>();
    }

    internal void Initialize(Transform attackOwner, Action hitFeedback)
    {
        owner = attackOwner;
        playHitFeedback = hitFeedback;
    }

    internal void BeginAttack()
    {
        hitboxCollider.enabled = false;
        damagedEnemies.Clear();
        feedbackPlayed = false;
    }

    internal void EndAttack()
    {
        hitboxCollider.enabled = false;
        damagedEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponentInParent<EnemyController>();
        if (enemy == null || !damagedEnemies.Add(enemy))
            return;

        Vector2 sourcePosition = owner != null
            ? owner.position
            : transform.position;

        if (!enemy.TryTakeDamage(sourcePosition, Mathf.Max(1, damage)))
            return;

        if (feedbackPlayed)
            return;

        feedbackPlayed = true;
        playHitFeedback?.Invoke();
    }
}
