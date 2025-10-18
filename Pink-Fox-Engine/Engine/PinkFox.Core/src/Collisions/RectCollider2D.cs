using System.Numerics;

namespace PinkFox.Core.Collisions;

public class RectCollider2D : ICollider2D
{
    public Vector2 Position { get; private set; }
    public Vector2 Scale { get; private set; }

    public float HalfWidth => Scale.X / 2f;
    public float HalfHeight => Scale.Y / 2f;

    public float Left => Position.X - HalfWidth;
    public float Right => Position.X + HalfWidth;
    public float Top => Position.Y - HalfHeight;
    public float Bottom => Position.Y + HalfHeight;

    public RectCollider2D(float x, float y, float width, float height)
    {
        Position = new(x, y);
        Scale = new(width, height);
    }

    public RectCollider2D(float x, float y, Vector2 scale)
    {
        Position = new(x, y);
        Scale = scale;
    }

    public RectCollider2D(Vector2 position, float width, float height)
    {
        Position = position;
        Scale = new(width, height);
    }

    public RectCollider2D(Vector2 position, Vector2 scale)
    {
        Position = position;
        Scale = scale;
    }

    public void AdjustPosition(Vector2 amount)
    {
        Position += amount;
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    public void AdjustScale(Vector2 amount)
    {
        Scale += amount;
    }

    public void SetScale(Vector2 scale)
    {
        Scale = scale;
    }

    public bool IsCollidingWith(ICollider2D collider)
    {
        return CollisionCheck.CheckForCollision(this, collider);
    }
}