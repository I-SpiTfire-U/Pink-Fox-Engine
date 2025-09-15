using System.Numerics;
using SDL;

namespace PinkFox.Core.Collisions;

public interface ICollider2D
{
    Vector2 Position { get; }

    public float HalfWidth { get; }
    public float HalfHeight { get; }

    float Left { get; }
    float Right { get; }
    float Top { get; }
    float Bottom { get; }

    void AdjustPosition(Vector2 amount);
    void SetPosition(Vector2 position);
    bool IsCollidingWith(ICollider2D collider);
}