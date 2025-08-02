namespace PinkFox.Core.Collisions;

public readonly struct CircleCollider(float centerX, float centerY, float radius) : ICollider
{
    public float CenterX { get; init; } = centerX;
    public float CenterY { get; init; } = centerY;
    public float Radius { get; init; } = radius;

    public float Top { get; init; } = centerY - radius;
    public float Bottom { get; init; } = centerY + radius;
    public float Left { get; init; } = centerX - radius;
    public float Right { get; init; } = centerX + radius;

    public bool IsCollidingWith(ICollider collider)
    {
        return collider switch
        {
            CircleCollider circle => Collision.CircleOnCircleCollision(this, circle),
            RectCollider rect => Collision.CircleOnRectCollision(this, rect),
            _ => false
        };
    }
}