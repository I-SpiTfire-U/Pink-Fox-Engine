using System.Numerics;

namespace PinkFox.Core.Collisions;

public interface ICollider
{
    Vector2 Center { get; init; }

    float Top { get; init; }
    float Bottom { get; init; }
    float Left { get; init; }
    float Right { get; init; }

    bool IsCollidingWith(ICollider other);
}