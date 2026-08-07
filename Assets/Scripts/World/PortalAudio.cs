using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalAudio : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource portalAudio;

    private void Awake()
    {
        if (portalAudio == null)
            portalAudio = GetComponent<AudioSource>();

        if (portalAudio != null)
            portalAudio.playOnAwake = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayPortalSound();
        }
    }

    private void PlayPortalSound()
    {
        if (portalAudio == null) return;

        portalAudio.Stop();
        portalAudio.Play();
    }
}
