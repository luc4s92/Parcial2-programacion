using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource winAudio;
    [SerializeField] private float delayBeforeVictory = 0.6f;

    private bool triggered = false;

    public void Play()
    {
        Debug.Log("Volver a Jugar...");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Exit()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        if (triggered) return;

        triggered = true;

        
        if (winAudio != null)
        {
            winAudio.Stop();
            winAudio.Play();
        }

       
        StartCoroutine(TriggerVictory());
    }

    private IEnumerator TriggerVictory()
    {
        yield return new WaitForSeconds(delayBeforeVictory);

       
        GameManager.Instance.SetVictory();
    }
}
