namespace PinkFox.Core.Collisions;

public interface ICollider
{
    float CenterX { get; init; }
    float CenterY { get; init; }

    float Top { get; init; }
    float Bottom { get; init; }
    float Left { get; init; }
    float Right { get; init; }

    bool IsCollidingWith(ICollider other);
}