using System.Numerics;

namespace PinkFox.Core.Collisions;

public static class Collision
{
    public static bool CircleOnCircleCollision(CircleCollider c1, CircleCollider c2)
    {
        float dx = c1.CenterX - c2.CenterX;
        float dy = c1.CenterY - c2.CenterY;
        float distanceSquared = dx * dx + dy * dy;
        float radiusSum = c1.Radius + c2.Radius;
        return distanceSquared <= radiusSum * radiusSum;
    }

    public static bool RectOnRectCollision(RectCollider a, RectCollider b)
    {
        return a.X < b.X + b.Width && a.X + a.Width > b.X && a.Y < b.Y + b.Height && a.Y + a.Height > b.Y;
    }

    public static bool CircleOnRectCollision(CircleCollider circle, RectCollider box)
    {
        float closestX = Math.Clamp(circle.CenterX, box.X, box.X + box.Width);
        float closestY = Math.Clamp(circle.CenterY, box.Y, box.Y + box.Height);

        float dx = circle.CenterX - closestX;
        float dy = circle.CenterY - closestY;

        return (dx * dx + dy * dy) <= (circle.Radius * circle.Radius);
    }

    public static Vector2 GetDeltaTo(ICollider c1, ICollider c2, bool normalize = false)
    {
        float dx = c1.CenterX - c2.CenterX;
        float dy = c1.CenterY - c2.CenterY;

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
            RectCollider rect => (rect.X, rect.X + rect.Width, rect.Y, rect.Y + rect.Height),
            CircleCollider circle => (circle.CenterX - circle.Radius, circle.CenterX + circle.Radius, circle.CenterY - circle.Radius, circle.CenterY + circle.Radius),
            _ => throw new NotSupportedException("Unsupported collider type")
        };
    }
    
    public static CollisionDirection GetCollisionDirection(ICollider a, ICollider b)
    {
        // Get centers
        float ax = a.CenterX;
        float ay = a.CenterY;
        float bx = b.CenterX;
        float by = b.CenterY;

        // Estimate bounding boxes for both
        (float aLeft, float aRight, float aTop, float aBottom) = GetBounds(a);
        (float bLeft, float bRight, float bTop, float bBottom) = GetBounds(b);

        // Compute overlap on each axis
        float dx = (aRight + aLeft) / 2f - (bRight + bLeft) / 2f;
        float dy = (aBottom + aTop) / 2f - (bBottom + bTop) / 2f;

        float combinedHalfWidths = (aRight - aLeft) / 2f + (bRight - bLeft) / 2f;
        float combinedHalfHeights = (aBottom - aTop) / 2f + (bBottom - bTop) / 2f;

        float overlapX = combinedHalfWidths - MathF.Abs(dx);
        float overlapY = combinedHalfHeights - MathF.Abs(dy);

        // Determine smallest overlap direction
        if (overlapX < overlapY)
        {
            return dx < 0 ? CollisionDirection.Right : CollisionDirection.Left;
        }
        return dy < 0 ? CollisionDirection.Bottom : CollisionDirection.Top;
    }

    private static float Clamp(float val, float min, float max) => val < min ? min : val > max ? max : val;
}