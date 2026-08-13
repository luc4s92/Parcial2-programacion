using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class CameraController : MonoBehaviour
{
    [Header("Objetivo")]
    [FormerlySerializedAs("objetive")]
    [SerializeField] private Transform target;
    [FormerlySerializedAs("movement")]
    [SerializeField] private Vector3 offset = new(0f, 1.25f, -10f);

    [Header("Seguimiento")]
    [Min(0.01f)]
    [SerializeField] private float horizontalSmoothTime = 0.16f;
    [Min(0.01f)]
    [SerializeField] private float verticalSmoothTime = 0.22f;
    [SerializeField] private Vector2 deadZone = new(0.8f, 0.45f);

    [Header("Anticipacion horizontal")]
    [Min(0f)]
    [SerializeField] private float lookAheadDistance = 1.35f;
    [Min(0f)]
    [SerializeField] private float lookAheadSpeedThreshold = 0.15f;
    [Min(0.01f)]
    [SerializeField] private float lookAheadSmoothTime = 0.25f;

    [Header("Limites opcionales")]
    [Tooltip("Collider que delimita la habitacion. Dejar vacio para seguimiento libre.")]
    [SerializeField] private Collider2D movementBounds;

    private Camera attachedCamera;
    private Vector3 previousTargetPosition;
    private float horizontalVelocity;
    private float verticalVelocity;
    private float currentLookAhead;
    private float lookAheadVelocity;
    private bool hasPreviousTargetPosition;

    public bool HasTarget => target != null;

    private void Awake()
    {
        attachedCamera = GetComponent<Camera>();

        if (target != null)
            SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f)
            return;

        UpdateLookAhead(deltaTime);

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.x += currentLookAhead;

        Vector3 currentPosition = transform.position;
        float desiredX = ApplyDeadZone(currentPosition.x, desiredPosition.x, deadZone.x);
        float desiredY = ApplyDeadZone(currentPosition.y, desiredPosition.y, deadZone.y);

        Vector3 nextPosition = new(
            Mathf.SmoothDamp(
                currentPosition.x,
                desiredX,
                ref horizontalVelocity,
                horizontalSmoothTime,
                Mathf.Infinity,
                deltaTime
            ),
            Mathf.SmoothDamp(
                currentPosition.y,
                desiredY,
                ref verticalVelocity,
                verticalSmoothTime,
                Mathf.Infinity,
                deltaTime
            ),
            desiredPosition.z
        );

        transform.position = ClampToBounds(nextPosition);
        previousTargetPosition = target.position;
    }

    public void Configure(Transform newTarget, Vector3 newOffset, bool snapImmediately = true)
    {
        target = newTarget;
        offset = newOffset;
        ResetTracking();

        if (snapImmediately && target != null)
            SnapToTarget();
    }

    public void SetTarget(Transform newTarget, bool snapImmediately = true)
    {
        Configure(newTarget, offset, snapImmediately);
    }

    public void SetMovementBounds(Collider2D bounds)
    {
        movementBounds = bounds;

        if (target != null)
            transform.position = ClampToBounds(transform.position);
    }

    private void UpdateLookAhead(float deltaTime)
    {
        if (!hasPreviousTargetPosition)
        {
            previousTargetPosition = target.position;
            hasPreviousTargetPosition = true;
        }

        float horizontalSpeed =
            (target.position.x - previousTargetPosition.x) / deltaTime;
        float desiredLookAhead = Mathf.Abs(horizontalSpeed) >= lookAheadSpeedThreshold
            ? Mathf.Sign(horizontalSpeed) * lookAheadDistance
            : 0f;

        currentLookAhead = Mathf.SmoothDamp(
            currentLookAhead,
            desiredLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime,
            Mathf.Infinity,
            deltaTime
        );
    }

    private void SnapToTarget()
    {
        ResetTracking();
        transform.position = ClampToBounds(target.position + offset);
    }

    private void ResetTracking()
    {
        horizontalVelocity = 0f;
        verticalVelocity = 0f;
        currentLookAhead = 0f;
        lookAheadVelocity = 0f;
        hasPreviousTargetPosition = false;
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        if (movementBounds == null || attachedCamera == null || !attachedCamera.orthographic)
            return position;

        Bounds bounds = movementBounds.bounds;
        float halfHeight = attachedCamera.orthographicSize;
        float halfWidth = halfHeight * attachedCamera.aspect;

        position.x = ClampAxis(position.x, bounds.min.x + halfWidth, bounds.max.x - halfWidth);
        position.y = ClampAxis(position.y, bounds.min.y + halfHeight, bounds.max.y - halfHeight);
        return position;
    }

    private static float ApplyDeadZone(float current, float desired, float size)
    {
        float halfSize = Mathf.Max(0f, size) * 0.5f;
        float difference = desired - current;

        if (Mathf.Abs(difference) <= halfSize)
            return current;

        return desired - Mathf.Sign(difference) * halfSize;
    }

    private static float ClampAxis(float value, float minimum, float maximum)
    {
        if (minimum > maximum)
            return (minimum + maximum) * 0.5f;

        return Mathf.Clamp(value, minimum, maximum);
    }

    private void OnValidate()
    {
        horizontalSmoothTime = Mathf.Max(0.01f, horizontalSmoothTime);
        verticalSmoothTime = Mathf.Max(0.01f, verticalSmoothTime);
        lookAheadSmoothTime = Mathf.Max(0.01f, lookAheadSmoothTime);
        deadZone.x = Mathf.Max(0f, deadZone.x);
        deadZone.y = Mathf.Max(0f, deadZone.y);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.9f);
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZone.x, deadZone.y, 0f));

        if (movementBounds == null)
            return;

        Gizmos.color = new Color(1f, 0.75f, 0.2f, 0.9f);
        Gizmos.DrawWireCube(movementBounds.bounds.center, movementBounds.bounds.size);
    }

}
