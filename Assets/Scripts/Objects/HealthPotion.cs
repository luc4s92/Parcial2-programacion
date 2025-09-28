
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class HealthPotion : Item
{
    public override void Use()
    {
        Debug.Log("Curando al jugador");
        // Lógica para curar al jugador
    }
}