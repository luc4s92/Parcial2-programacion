using UnityEngine;

internal sealed class EnemyAttackTelegraph
{
    private const float MinimumBlend = 0.35f;

    private readonly SpriteRenderer spriteRenderer;
    private readonly Color warningColor;
    private readonly float pulseSpeed;

    private Color baseColor;
    private float elapsedTime;
    private bool isActive;

    internal EnemyAttackTelegraph(
        SpriteRenderer spriteRenderer,
        Color warningColor,
        float pulseSpeed)
    {
        this.spriteRenderer = spriteRenderer;
        this.warningColor = warningColor;
        this.pulseSpeed = Mathf.Max(0.1f, pulseSpeed);
    }

    internal void Begin()
    {
        if (spriteRenderer == null)
            return;

        baseColor = spriteRenderer.color;
        elapsedTime = 0f;
        isActive = true;
        spriteRenderer.color = Color.Lerp(baseColor, warningColor, MinimumBlend);
    }

    internal void Tick(float deltaTime)
    {
        if (!isActive || spriteRenderer == null)
            return;

        elapsedTime += deltaTime;
        float pulse = Mathf.PingPong(elapsedTime * pulseSpeed, 1f);
        float blend = Mathf.Lerp(MinimumBlend, 1f, pulse);
        spriteRenderer.color = Color.Lerp(baseColor, warningColor, blend);
    }

    internal void End()
    {
        if (!isActive)
            return;

        if (spriteRenderer != null)
            spriteRenderer.color = baseColor;

        isActive = false;
    }
}
