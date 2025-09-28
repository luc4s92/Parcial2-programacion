using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxLife = 3;

    private int currentLife;
    private bool isDead;

    public int Life => currentLife;
    public int MaxLife => maxLife;
    public bool IsAlive => !isDead;

    // Evento: vida actual y máxima + dirección del ataque
    public event Action<int, int, Vector2> OnLifeChanged;

    public event Action OnDeath;

    private void Awake()
    {
        currentLife = maxLife;
        OnLifeChanged?.Invoke(currentLife, maxLife, Vector2.zero);
    }

    // Implementación exacta de IDamageable
    public void TakeDamage(int damage, Vector2 direction)
    {
        if (isDead) return;

        currentLife -= damage;

        // Disparar evento de vida
        OnLifeChanged?.Invoke(currentLife, maxLife, direction);

        // Comprobar muerte
        if (currentLife <= 0 && !isDead)
        {
            isDead = true;
            OnDeath?.Invoke();
        }
    }

    public void RestoreFullHealth()
    {
        currentLife = maxLife;
        isDead = false;
        OnLifeChanged?.Invoke(currentLife, maxLife, Vector2.zero);
    }
}
