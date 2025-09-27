public interface IDamageable
{
    int Life { get; }
    bool IsAlive { get; }

    void TakeDamage(int damage, UnityEngine.Vector2 direction);
}
