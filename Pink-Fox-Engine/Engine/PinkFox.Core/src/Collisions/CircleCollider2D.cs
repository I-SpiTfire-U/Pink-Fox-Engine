using System.Numerics;

namespace PinkFox.Core.Collisions;

public class CircleCollider2D : ICollider2D
{
    public float Radius { get; private set; }
    public Vector2 Position { get; private set; }

    public float HalfWidth => Radius / 2f;
    public float HalfHeight => Radius / 2f;

    public float Left => Position.X - HalfWidth;
    public float Right => Position.X + HalfWidth;
    public float Top => Position.Y - HalfHeight;
    public float Bottom => Position.Y + HalfHeight;

    public CircleCollider2D(float x, float y, float radius)
    {
        Position = new(x, y);
        Radius = radius;
    }

    public CircleCollider2D(Vector2 position, float radius)
    {
        Position = position;
        Radius = radius;
    }

    public void AdjustPosition(Vector2 amount)
    {
        Position += amount;
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    public void AdjustRadius(float amount)
    {
        Radius += amount;
    }

    public void SetRadius(float radius)
    {
        Radius = radius;
    }

    public bool IsCollidingWith(ICollider2D collider)
    {
        return CollisionCheck.CheckForCollision(this, collider);
    }
}