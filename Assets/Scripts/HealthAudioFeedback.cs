using UnityEngine;
using UnityEngine.Audio;

public class HealthAudioFeedback : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer gameMixer;

    [Header("Referencia al Health del jugador")]
    [SerializeField] private Health playerHealth;

    private void Start()
    {
        if (playerHealth != null)
        {
            // Suscribir al evento de cambio de vida
            playerHealth.OnLifeChanged += UpdateAudioBasedOnHealth;
        }
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnLifeChanged -= UpdateAudioBasedOnHealth;
        }
    }

    private void UpdateAudioBasedOnHealth(int currentLife, int maxLife, Vector2 dir)
    {
        // Convertir vida a rango 0..1
        float normalized = (float)currentLife / maxLife;

        // Mapeo lineal a frecuencia del lowpass
        float lowpassValue = Mathf.Lerp(400f, 22000f, normalized);

        // Aplicar al mixer
        gameMixer.SetFloat("MasterLowpassFreq", lowpassValue);

        Debug.Log($"[HealthAudioFeedback] Vida:{currentLife}/{maxLife} | LPF:{lowpassValue}");
    }
}
