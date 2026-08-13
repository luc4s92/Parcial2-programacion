using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
public sealed class ResizablePlatform2D : MonoBehaviour
{
    [SerializeField, Min(1)] private int widthInTiles = 4;
    [SerializeField, Min(0.01f)] private float tileSize = 1f;
    [SerializeField, Range(0.05f, 1f)] private float colliderHeight = 0.2f;

    private void Reset()
    {
        ScheduleSizeUpdate();
    }

    private void Start()
    {
        ApplySize();
    }

    private void OnValidate()
    {
        ClampSettings();
        ScheduleSizeUpdate();
    }

    [ContextMenu("Aplicar tamano")]
    public void ApplySize()
    {
        ClampSettings();

        float width = widthInTiles * tileSize;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.drawMode = SpriteDrawMode.Tiled;
        spriteRenderer.tileMode = SpriteTileMode.Continuous;
        spriteRenderer.size = new Vector2(width, tileSize);

        BoxCollider2D platformCollider = GetComponent<BoxCollider2D>();
        platformCollider.size = new Vector2(width, colliderHeight);
        platformCollider.offset = new Vector2(
            0f,
            (tileSize - colliderHeight) * 0.5f
        );
    }

    private void ClampSettings()
    {
        widthInTiles = Mathf.Max(1, widthInTiles);
        tileSize = Mathf.Max(0.01f, tileSize);
        colliderHeight = Mathf.Clamp(colliderHeight, 0.05f, tileSize);
    }

    private void ScheduleSizeUpdate()
    {
#if UNITY_EDITOR
        EditorApplication.delayCall -= ApplySizeAfterValidation;
        EditorApplication.delayCall += ApplySizeAfterValidation;
#endif
    }

#if UNITY_EDITOR
    private void ApplySizeAfterValidation()
    {
        EditorApplication.delayCall -= ApplySizeAfterValidation;

        if (this == null || Application.isPlaying)
            return;

        ApplySize();
    }

    private void OnDestroy()
    {
        EditorApplication.delayCall -= ApplySizeAfterValidation;
    }
#endif
}
