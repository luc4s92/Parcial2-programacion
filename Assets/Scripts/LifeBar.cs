using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] private Image refillLifeBar;
    [SerializeField] private PlayerMovement playerMovement;
    private float fullHealth;
    void Start()
    {
       playerMovement = GameObject.Find("Player") .GetComponent<PlayerMovement>();
        fullHealth = playerMovement.Life;
    }

    // Update is called once per frame
    void Update()
    {
        refillLifeBar.fillAmount = playerMovement.Life / fullHealth;
    }
}
