using UnityEngine;

public class IdleStrategy : IEnemyStrategy
{
    public void Execute(IEnemyContext context)
    {
        context.Rigidbody.velocity = Vector2.zero;
    }
}
