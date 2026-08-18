using UnityEngine;

public sealed class EnemyAudio : MonoBehaviour
{
    [Header("Clips de Sonido")]
    [SerializeField] private AudioSource hitSFX;
    [SerializeField] private AudioSource deathSFX;

    public void PlayHit()
    {
        hitSFX?.Play();
    }

    public void PlayDeath()
    {
        if (deathSFX != null)
        {
            deathSFX.Play();
            return;
        }

        hitSFX?.Play();
    }
}
