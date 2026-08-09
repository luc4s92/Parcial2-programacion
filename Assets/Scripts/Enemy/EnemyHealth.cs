internal sealed class EnemyHealth
{
    internal int CurrentLife { get; private set; }
    internal bool IsAlive => CurrentLife > 0;

    internal EnemyHealth(int maxLife)
    {
        CurrentLife = maxLife;
    }

    internal void TakeDamage(int damage)
    {
        if (!IsAlive || damage <= 0) return;

        CurrentLife = System.Math.Max(CurrentLife - damage, 0);
    }
}
