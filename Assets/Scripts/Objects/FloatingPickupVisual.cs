using UnityEngine;

public sealed class FloatingPickupVisual : MonoBehaviour
{
    [SerializeField] private Transform visual;
    [SerializeField] private float bobHeight = 0.12f;
    [SerializeField] private float bobSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 initialLocalPosition;

    private void Awake()
    {
        if (visual == null)
            visual = transform;

        initialLocalPosition = visual.localPosition;
    }

    private void Update()
    {
        float offset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        visual.localPosition = initialLocalPosition + Vector3.up * offset;
        visual.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}
