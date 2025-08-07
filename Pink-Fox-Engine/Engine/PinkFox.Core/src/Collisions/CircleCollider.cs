using System.Numerics;

namespace PinkFox.Core.Collisions;

public readonly struct CircleCollider : ICollider
{
    public Vector2 Center { get; init; }
    public float Radius { get; init; }

    public float Top { get; init; }
    public float Bottom { get; init; }
    public float Left { get; init; }
    public float Right { get; init; }

    public CircleCollider(float x, float y, float radius)
    {
        Center = new(x, y);
        Radius = radius;

        Left = Center.X - radius;
        Right = Center.X + radius;
        Top = Center.Y - radius;
        Bottom = Center.Y + radius;
    }

    public CircleCollider(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;

        Left = Center.X - radius;
        Right = Center.X + radius;
        Top = Center.Y - radius;
        Bottom = Center.Y + radius;
    }

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