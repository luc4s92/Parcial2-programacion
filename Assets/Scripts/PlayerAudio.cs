using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
   [SerializeField] private AudioSource swingSFX;
   [SerializeField] private AudioSource hitSFX;
   [SerializeField] private AudioSource damageSFX;
   [SerializeField] private AudioSource deathSFX;

    public void PlaySwing()
    {
        swingSFX?.Play();
    }

    public void PlayHit()
    {
        hitSFX?.Play();
    }

    public void PlayDamage()
    {
        damageSFX?.Play();
    }

    public void PlayDeath()
    {
        if (deathSFX != null)
        {
            deathSFX.Play();
            return;
        }

        damageSFX?.Play();
    }

  

}
