using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Clips de Sonido")]
    [SerializeField] private AudioSource hitSFX;
    [SerializeField] private AudioSource deathSFX;



    public void PlayHit()
    {
        hitSFX.Play();
    }

    public void PlayDeath()
    {
        deathSFX.Play();
    }
}
