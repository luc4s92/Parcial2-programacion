using UnityEngine;

public class HealthAudio : MonoBehaviour
{
    [Header("LowPass Filter del Player")]
    public AudioLowPassFilter lowPass;

    [Header("Referencia al Health del jugador")]
    public Health playerHealth;

    private void Start()
    {
        playerHealth.OnLifeChanged += UpdateAudioBasedOnHealth;
    }

    private void OnDestroy()
    {
        playerHealth.OnLifeChanged -= UpdateAudioBasedOnHealth;
    }

    private void UpdateAudioBasedOnHealth(int currentLife, int maxLife, Vector2 dir)
    {
        float normalized = (float)currentLife / maxLife;

        float cutoff = Mathf.Lerp(400f, 22000f, normalized);

        lowPass.cutoffFrequency = cutoff;

        Debug.Log($"[HealthAudioFeedback] Vida:{currentLife}/{maxLife} | LPF:{cutoff}");
    }
}
