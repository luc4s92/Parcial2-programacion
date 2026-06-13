using System.Collections;
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

    [Header("Derrota")]
    [SerializeField] private string loserSceneName = "LoserScreen";
    [SerializeField] private float loserSceneDelay = 4f;

    // ----------------- Referencias -----------------
    [SerializeField] private Transform player;
    private Health playerHealth;
    private Coroutine loserSceneRoutine;

    // Lista de enemigos activos
    private List<EnemyController> activeEnemies = new List<EnemyController>();

    // Contador de enemigos muertos
    private int enemiesKilled = 0;

    private void Start()
    {
        if (player != null)
        {
            RegisterPlayer(player.GetComponent<Health>());
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnDeath -= OnPlayerDeath;
    }

    public void RegisterPlayer(Health health)
    {
        if (health == null || playerHealth == health) return;

        if (playerHealth != null)
            playerHealth.OnDeath -= OnPlayerDeath;

        playerHealth = health;
        playerHealth.OnDeath += OnPlayerDeath;
        CurrentState = GameState.Playing;
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
        if (CurrentState == GameState.GameOver) return;

        CurrentState = GameState.GameOver;
        Debug.Log("Game Over");

        // Avisar a los enemigos que el player murio
        for (int i = activeEnemies.Count - 1; i >= 0; i--)
        {
            if (activeEnemies[i] == null)
            {
                activeEnemies.RemoveAt(i);
                continue;
            }

            activeEnemies[i].NotifyPlayerDeath();
        }

        if (loserSceneRoutine != null)
            StopCoroutine(loserSceneRoutine);

        loserSceneRoutine = StartCoroutine(LoadLoserScreenAfterDelay());
    }

    private IEnumerator LoadLoserScreenAfterDelay()
    {
        yield return new WaitForSecondsRealtime(loserSceneDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(loserSceneName);
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
