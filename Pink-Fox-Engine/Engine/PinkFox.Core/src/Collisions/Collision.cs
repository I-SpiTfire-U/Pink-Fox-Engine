using System.Numerics;

namespace PinkFox.Core.Collisions;

public static class Collision
{
    public static bool CircleOnCircleCollision(CircleCollider c1, CircleCollider c2)
    {
        float dx = c1.Center.X - c2.Center.X;
        float dy = c1.Center.Y - c2.Center.Y;
        float distanceSquared = dx * dx + dy * dy;
        float radiusSum = c1.Radius + c2.Radius;
        return distanceSquared <= radiusSum * radiusSum;
    }

    public static bool RectOnRectCollision(RectCollider a, RectCollider b)
    {
        return a.Position.X < b.Position.X + b.Size.X && a.Position.X + a.Size.X > b.Position.X && a.Position.Y < b.Position.Y + b.Size.Y && a.Position.Y + a.Size.Y > b.Position.Y;
    }

    public static bool CircleOnRectCollision(CircleCollider circle, RectCollider box)
    {
        float closestX = Math.Clamp(circle.Center.X, box.Position.X, box.Position.X + box.Size.X);
        float closestY = Math.Clamp(circle.Center.Y, box.Position.Y, box.Position.Y + box.Size.Y);

        float dx = circle.Center.X - closestX;
        float dy = circle.Center.Y - closestY;

        return (dx * dx + dy * dy) <= (circle.Radius * circle.Radius);
    }

    public static Vector2 GetDeltaTo(ICollider c1, ICollider c2, bool normalize = false)
    {
        float dx = c1.Center.X - c2.Center.X;
        float dy = c1.Center.Y - c2.Center.Y;

        if (!normalize)
        {
            return new(dx, dy);
        }

        float length = MathF.Sqrt(dx * dx + dy * dy);
        return length > 0 ? new(dx / length, dy / length) : new(0, 0);
    }

    private static (float Left, float Right, float Top, float Bottom) GetBounds(ICollider collider)
    {
        return collider switch
        {
            RectCollider rect => (rect.Position.X, rect.Position.X + rect.Size.X, rect.Position.Y, rect.Position.Y + rect.Size.Y),
            CircleCollider circle => (circle.Center.X - circle.Radius, circle.Center.X + circle.Radius, circle.Center.Y - circle.Radius, circle.Center.Y + circle.Radius),
            _ => throw new NotSupportedException("Unsupported collider type")
        };
    }
    
    public static CollisionDirection GetCollisionDirection(ICollider a, ICollider b)
    {
        float ax = a.Center.X;
        float ay = a.Center.Y;
        float bx = b.Center.X;
        float by = b.Center.Y;

        (float aLeft, float aRight, float aTop, float aBottom) = GetBounds(a);
        (float bLeft, float bRight, float bTop, float bBottom) = GetBounds(b);

        float dx = (aRight + aLeft) / 2f - (bRight + bLeft) / 2f;
        float dy = (aBottom + aTop) / 2f - (bBottom + bTop) / 2f;

        float combinedHalfWidths = (aRight - aLeft) / 2f + (bRight - bLeft) / 2f;
        float combinedHalfHeights = (aBottom - aTop) / 2f + (bBottom - bTop) / 2f;

        float overlapX = combinedHalfWidths - MathF.Abs(dx);
        float overlapY = combinedHalfHeights - MathF.Abs(dy);

        if (overlapX < overlapY)
        {
            return dx < 0 ? CollisionDirection.Right : CollisionDirection.Left;
        }
        return dy < 0 ? CollisionDirection.Bottom : CollisionDirection.Top;
    }
}