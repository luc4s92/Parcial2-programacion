using UnityEngine;

public class OneShotVoiceLine : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource voiceSource;

    [Header("Opcional")]
    [SerializeField] private float destroyDelay = 1f;
    [SerializeField] private bool destroyAfterPlaying = true;

    private void Awake()
    {
        if (voiceSource == null)
            voiceSource = GetComponent<AudioSource>();

        if (voiceSource != null)
            voiceSource.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            PlayVoice();
    }

    private void PlayVoice()
    {
        if (voiceSource == null) return;

        voiceSource.Stop();
        voiceSource.Play();

        if (destroyAfterPlaying)
            Destroy(gameObject, destroyDelay);
    }
}
