using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicTrigger : MonoBehaviour
{
    public AudioMixerSnapshot onEnterSnapshot;
    public AudioMixerSnapshot onExitSnapshot;
    public float transitionTime = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered music trigger.");
            onEnterSnapshot.TransitionTo(transitionTime);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            onExitSnapshot.TransitionTo(transitionTime);
    }
}
