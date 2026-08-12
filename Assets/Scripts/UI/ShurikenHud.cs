using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShurikenHud : MonoBehaviour
{
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private RectTransform chargeContainer;
    [SerializeField] private Image chargeTemplate;
    [SerializeField] private Color availableColor = new(0.25f, 0.82f, 0.95f, 1f);
    [SerializeField] private Color emptyColor = new(0.12f, 0.18f, 0.22f, 0.85f);

    private readonly List<Image> chargeIndicators = new();
    private IShurikenInventory inventory;

    private void OnEnable()
    {
        inventory = PlayerProgression.Shuriken;
        inventory.Changed += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.Changed -= Refresh;

        inventory = null;
    }

    private void Refresh()
    {
        if (inventory == null || contentRoot == null)
            return;

        contentRoot.SetActive(inventory.IsUnlocked);
        if (!inventory.IsUnlocked)
            return;

        EnsureIndicatorCount(inventory.MaxCharges);

        for (int index = 0; index < chargeIndicators.Count; index++)
        {
            Image indicator = chargeIndicators[index];
            bool belongsToCapacity = index < inventory.MaxCharges;
            indicator.gameObject.SetActive(belongsToCapacity);

            if (belongsToCapacity)
                indicator.color = index < inventory.CurrentCharges
                    ? availableColor
                    : emptyColor;
        }
    }

    private void EnsureIndicatorCount(int requiredCount)
    {
        if (chargeTemplate == null || chargeContainer == null)
            return;

        if (chargeIndicators.Count == 0)
            chargeIndicators.Add(chargeTemplate);

        while (chargeIndicators.Count < requiredCount)
        {
            Image indicator = Instantiate(chargeTemplate, chargeContainer);
            indicator.name = $"Charge_{chargeIndicators.Count + 1}";
            chargeIndicators.Add(indicator);
        }
    }
}
