using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{  public void Play()
    {
        Debug.Log("Volver a Jugar...");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex );
    }

    public void Exit()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene("WinnerScreen");
        }
    }


  
}
