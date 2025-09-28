using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private int maxLife = 5;
    [SerializeField] private int currentLife;

    public int Life => currentLife;
    public int MaxLife => maxLife;
    public bool IsAlive => currentLife > 0;

    // Eventos
    public event Action<int, int, Vector2> OnLifeChanged; // vidaActual, vidaMax, dirección del daño
    public event Action OnDeath;

    private void Awake()
    {
        currentLife = maxLife;
    }

    // ----------------- Daño -----------------
    public void TakeDamage(int damage, Vector2 attackDirection)
    {
        if (!IsAlive) return;

        currentLife -= damage;
        currentLife = Mathf.Max(currentLife, 0);

        OnLifeChanged?.Invoke(currentLife, maxLife, attackDirection);

        if (currentLife <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    // ----------------- Curación -----------------
    public void Heal(int amount)
    {
        if (!IsAlive) return; // no curar si está muerto

        currentLife += amount;
        currentLife = Mathf.Min(currentLife, maxLife);

        Debug.Log($"[Health] Curado +{amount}. Vida actual: {currentLife}/{maxLife}");

        // notificar para actualizar UI
        OnLifeChanged?.Invoke(currentLife, maxLife, Vector2.zero);
    }

    // ----------------- Reset -----------------
    public void ResetLife()
    {
        currentLife = maxLife;
        OnLifeChanged?.Invoke(currentLife, maxLife, Vector2.zero);
    }
}
