using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(PlatformEffector2D))]
public sealed class OneWayPlatform : MonoBehaviour
{
    private void Reset()
    {
        ConfigureComponents();
    }

    private void Awake()
    {
        ConfigureComponents();
    }

    private void OnValidate()
    {
        ConfigureComponents();
    }

    private void ConfigureComponents()
    {
        Collider2D platformCollider = GetComponent<Collider2D>();
        if (platformCollider == null)
        {
            Debug.LogError(
                "[OneWayPlatform] La plataforma necesita un Collider2D en el mismo GameObject.",
                this
            );
            return;
        }

        PlatformEffector2D platformEffector = GetComponent<PlatformEffector2D>();
        platformCollider.isTrigger = false;
        platformCollider.usedByEffector = true;
        platformEffector.useOneWay = true;
        platformEffector.useOneWayGrouping = true;
    }
}
