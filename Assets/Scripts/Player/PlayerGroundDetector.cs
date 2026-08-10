using UnityEngine;

internal sealed class PlayerGroundDetector
{
    private const int MaxGroundHits = 8;
    private const float OneWaySurfaceTolerance = 0.05f;

    private readonly Collider2D playerCollider;
    private readonly RaycastHit2D[] groundHits = new RaycastHit2D[MaxGroundHits];
    private ContactFilter2D contactFilter;

    internal PlayerGroundDetector(Collider2D playerCollider)
    {
        this.playerCollider = playerCollider;
        contactFilter = new ContactFilter2D
        {
            useTriggers = false
        };
    }

    internal bool TryGetGround(
        float distance,
        LayerMask floorLayer,
        float maxGroundAngle,
        float verticalSpeed,
        out RaycastHit2D groundHit)
    {
        contactFilter.SetLayerMask(floorLayer);

        int hitCount = playerCollider.Cast(
            Vector2.down,
            contactFilter,
            groundHits,
            distance,
            true
        );
        float minGroundNormalY = Mathf.Cos(
            Mathf.Clamp(maxGroundAngle, 0f, 89f) * Mathf.Deg2Rad
        );

        for (int i = 0; i < hitCount; i++)
        {
            RaycastHit2D hit = groundHits[i];
            if (hit.collider == null || hit.normal.y < minGroundNormalY)
                continue;

            if (!IsValidOneWayHit(hit.collider, verticalSpeed))
                continue;

            groundHit = hit;
            return true;
        }

        groundHit = default;
        return false;
    }

    private bool IsValidOneWayHit(Collider2D groundCollider, float verticalSpeed)
    {
        if (!groundCollider.TryGetComponent(out OneWayPlatform _))
            return true;

        bool isFallingOrStill = verticalSpeed <= 0f;
        bool feetAreAboveSurface =
            playerCollider.bounds.min.y >=
            groundCollider.bounds.max.y - OneWaySurfaceTolerance;

        return isFallingOrStill && feetAreAboveSurface;
    }
}
