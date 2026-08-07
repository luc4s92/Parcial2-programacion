using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioSource swingSFX;
    [SerializeField] private AudioSource hitSFX;
    [SerializeField] private AudioSource damageSFX;
    [SerializeField] private AudioSource deathSFX;

    internal void PlaySwing()
    {
        swingSFX?.Play();
    }

    internal void PlayHit()
    {
        hitSFX?.Play();
    }

    internal void PlayDamage()
    {
        damageSFX?.Play();
    }

    internal void PlayDeath()
    {
        if (deathSFX != null)
        {
            deathSFX.Play();
            return;
        }

        damageSFX?.Play();
    }

}
