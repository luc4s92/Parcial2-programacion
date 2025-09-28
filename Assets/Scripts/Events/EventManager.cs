using System;

public static class EventManager
{
    // Evento: vida actual y vida máxima del jugador
    public static event Action<int, int> OnPlayerLifeChanged;

    // Disparar evento
    public static void TriggerPlayerLifeChanged(int currentLife, int maxLife)
    {
        OnPlayerLifeChanged?.Invoke(currentLife, maxLife);
    }
}
