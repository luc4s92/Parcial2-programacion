using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelUp : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource portalAudio;
    [SerializeField] private float delayBeforeLoad = 0.6f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (triggered) return;

        triggered = true;

        // reproducir sonido
        if (portalAudio != null)
        {
            portalAudio.Stop();
            portalAudio.Play();
        }

        // cargar siguiente escena con delay
        StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
