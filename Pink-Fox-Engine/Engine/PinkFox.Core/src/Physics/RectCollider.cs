namespace PinkFox.Core.Physics;

public readonly struct RectCollider : ICollider
{
    public float X { get; init;}
    public float Y { get; init;}
    public float Width { get; init; }
    public float Height { get; init; }

    public float CenterX { get; init; }
    public float CenterY { get; init; }

    public float Left { get; init; }
    public float Right { get; init; }
    public float Top { get; init; }
    public float Bottom { get; init; }

    public RectCollider(float x, float y, float width, float height, float? centerX = null, float? centerY = null)
    {
        Width = width;
        Height = height;

        CenterX = centerX ?? x + width / 2;
        CenterY = centerY ?? y + height / 2;

        Left = CenterX - width / 2;
        Right = CenterX + width / 2;
        Top = CenterY - height / 2;
        Bottom = CenterY + height / 2;

        X = Left;
        Y = Top;
    }

    public bool IsCollidingWith(ICollider collider)
    {
        return collider switch
        {
            RectCollider rect => Collision.RectOnRectCollision(this, rect),
            CircleCollider circle => Collision.CircleOnRectCollision(circle, this),
            _ => false
        };
    }
}