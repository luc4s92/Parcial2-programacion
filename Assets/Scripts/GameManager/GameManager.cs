using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ----------------- Singleton -----------------
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // persiste entre escenas
    }

    // ----------------- Estados -----------------
    public enum GameState { Playing, Paused, GameOver, Victory }
    public GameState CurrentState { get; private set; } = GameState.Playing;

    // ----------------- Referencias -----------------
    [SerializeField] private Transform player;
    private Health playerHealth;

    // Lista de enemigos activos
    private List<EnemyController> activeEnemies = new List<EnemyController>();

    // Contador de enemigos muertos
    private int enemiesKilled = 0;

    private void Start()
    {
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.OnDeath += OnPlayerDeath;
        }
    }

    // ----------------- Enemigos -----------------
    public void RegisterEnemy(EnemyController enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
    }

    public void EnemyKilled(EnemyController enemy)
    {
        enemiesKilled++;
        Debug.Log($"[GameManager] Enemigos muertos: {enemiesKilled}");

        // Se puede desregistrar para no contar dos veces
        UnregisterEnemy(enemy);
    }

    // ----------------- Estados del juego -----------------
    private void OnPlayerDeath()
    {
        CurrentState = GameState.GameOver;
        Debug.Log("Game Over");

        // Avisar a los enemigos que el player murió
        foreach (var enemy in activeEnemies)
        {
            enemy.NotifyPlayerDeath();
        }

        // Cargar pantalla de derrota después de 2 segundos
        Invoke(nameof(LoadLoserScreen), 2f);
    }

    private void LoadLoserScreen()
    {
        SceneManager.LoadScene("LoserScreen");
    }

    public void SetVictory()
    {
        CurrentState = GameState.Victory;
        Debug.Log("Victory!");
        Debug.Log($"Enemigos derrotados: {enemiesKilled}");
        SceneManager.LoadScene("WinnerScreen");
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
        }
        else if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
        }
    }
}
