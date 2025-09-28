
using UnityEngine;

public interface IEnemyContext
{
    Transform Transform { get; }
    Rigidbody2D Rigidbody { get; }
    float Speed { get; }
    Transform Player { get; }
    float DetectionRadius { get; }
    bool IsTakingDamage { get; }
    bool IsAlive { get; }
    bool IsPlayerAlive { get; }
}