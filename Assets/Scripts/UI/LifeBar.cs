using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    [SerializeField] private Image refillLifeBar;

    private void OnEnable()
    {
        EventManager.OnPlayerLifeChanged += UpdateLifeBar;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerLifeChanged -= UpdateLifeBar;
    }

    private void UpdateLifeBar(int currentLife, int maxLife)
    {
        refillLifeBar.fillAmount = (float)currentLife / maxLife;
    }
}
