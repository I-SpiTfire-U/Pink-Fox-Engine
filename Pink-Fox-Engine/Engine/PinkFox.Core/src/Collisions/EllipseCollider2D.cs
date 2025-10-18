using System.Numerics;

namespace PinkFox.Core.Collisions;

public class EllipseCollider2D : ICollider2D
{
    public Vector2 Radius { get; private set; }
    public Vector2 Position { get; private set; }

    public float HalfWidth => Radius.X / 2f;
    public float HalfHeight => Radius.Y / 2f;

    public float Left => Position.X - HalfWidth;
    public float Right => Position.X + HalfWidth;
    public float Top => Position.Y - HalfHeight;
    public float Bottom => Position.Y + HalfHeight;

    public EllipseCollider2D(float x, float y, float radiusX, float radiusY)
    {
        Position = new(x, y);
        Radius = new(radiusX, radiusY);
    }

    public EllipseCollider2D(float x, float y, Vector2 radius)
    {
        Position = new(x, y);
        Radius = radius;
    }

    public EllipseCollider2D(Vector2 position, float radiusX, float radiusY)
    {
        Position = position;
        Radius = new(radiusX, radiusY);
    }

    public EllipseCollider2D(Vector2 position, Vector2 radius)
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

    public void AdjustRadius(Vector2 amount)
    {
        Radius += amount;
    }

    public void SetRadius(Vector2 radius)
    {
        Radius = radius;
    }

    public bool IsCollidingWith(ICollider2D collider)
    {
        return CollisionCheck.CheckForCollision(this, collider);
    }
}