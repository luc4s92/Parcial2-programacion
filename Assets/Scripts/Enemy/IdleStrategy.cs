using UnityEngine;

public class IdleStrategy : IEnemyStrategy
{
    public void Execute(IEnemyContext context)
    {
        context.Rigidbody.linearVelocity = Vector2.zero;
    }
}
