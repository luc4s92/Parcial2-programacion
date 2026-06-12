using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAudio : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Clips de Sonido")]
    [SerializeField] private AudioSource hitSFX;       

    public void PlayPickUp()
    {
        Debug.Log("[ItemAudio] Reproduciendo sonido de recogida.");
        if (hitSFX != null && hitSFX.clip != null)
            AudioSource.PlayClipAtPoint(hitSFX.clip, transform.position);
    }

}
