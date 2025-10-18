namespace PinkFox.Core.Collisions;

public static class CollisionCheck
{
    public static bool CheckForCollision(ICollider2D collider1, ICollider2D collider2)
    {
        if (collider1 is CircleCollider2D circle1 && collider2 is CircleCollider2D circle2)
        {
            return CircleOnCircleCollision(circle1, circle2);
        }

        if (collider1 is EllipseCollider2D ellipse1 && collider2 is EllipseCollider2D ellipse2)
        {
            return EllipseOnEllipseCollision(ellipse1, ellipse2);
        }

        if (collider1 is RectCollider2D rect1 && collider2 is RectCollider2D rect2)
        {
            return RectOnRectCollision(rect1, rect2);
        }

        if (collider1 is CircleCollider2D circle3 && collider2 is EllipseCollider2D ellipse3)
        {
            return CircleOnEllipseCollision(circle3, ellipse3);
        }

        if (collider1 is CircleCollider2D circle4 && collider2 is RectCollider2D rect3)
        {
            return CircleOnRectCollision(circle4, rect3);
        }

        if (collider1 is EllipseCollider2D ellipse4 && collider2 is RectCollider2D rect4)
        {
            return EllipseOnRectCollision(ellipse4, rect4);
        }

        return false;
    }

    public static bool CircleOnCircleCollision(CircleCollider2D c1, CircleCollider2D c2)
    {
        float dx = c1.Position.X - c2.Position.X;
        float dy = c1.Position.Y - c2.Position.Y;
        float distanceSquared = dx * dx + dy * dy;

        float radiusSum = c1.Radius + c2.Radius;

        return distanceSquared <= radiusSum * radiusSum;
    }

    public static bool EllipseOnEllipseCollision(EllipseCollider2D e1, EllipseCollider2D e2)
    {
        float dx = e1.Position.X - e2.Position.X;
        float dy = e1.Position.Y - e2.Position.Y;

        float rx = e1.Radius.X + e2.Radius.X;
        float ry = e1.Radius.Y + e2.Radius.Y;

        float nx = dx / rx;
        float ny = dy / ry;

        return (nx * nx + ny * ny) <= 1f;
    }

    public static bool RectOnRectCollision(RectCollider2D r1, RectCollider2D r2)
    {
        float dx = r1.Position.X - r2.Position.X;
        float dy = r1.Position.Y - r2.Position.Y;

        float rx = r1.Scale.X + r2.Scale.X;
        float ry = r1.Scale.Y + r2.Scale.Y;

        return Math.Abs(dx) <= rx && Math.Abs(dy) <= ry;
    }

    public static bool CircleOnEllipseCollision(CircleCollider2D circle, EllipseCollider2D ellipse)
    {
        float dx = circle.Position.X - ellipse.Position.X;
        float dy = circle.Position.Y - ellipse.Position.Y;

        float rx = circle.Radius + ellipse.Radius.X;
        float ry = circle.Radius + ellipse.Radius.Y;

        float nx = dx / rx;
        float ny = dy / ry;

        return (nx * nx + ny * ny) <= 1f;
    }

    public static bool CircleOnRectCollision(CircleCollider2D circle, RectCollider2D rect)
    {
        float halfWidth = rect.Scale.X;
        float halfHeight = rect.Scale.Y;

        float closestX = Math.Clamp(circle.Position.X, rect.Position.X - halfWidth, rect.Position.X + halfWidth);
        float closestY = Math.Clamp(circle.Position.Y, rect.Position.Y - halfHeight, rect.Position.Y + halfHeight);

        float dx = circle.Position.X - closestX;
        float dy = circle.Position.Y - closestY;

        return (dx * dx + dy * dy) <= circle.Radius * circle.Radius;
    }

    public static bool EllipseOnRectCollision(EllipseCollider2D ellipse, RectCollider2D rect)
    {
        float dx = rect.Position.X - ellipse.Position.X;
        float dy = rect.Position.Y - ellipse.Position.Y;

        float scaledHalfWidth = rect.Scale.X / ellipse.Radius.X;
        float scaledHalfHeight = rect.Scale.Y / ellipse.Radius.Y;
        float scaledDx = dx / ellipse.Radius.X;
        float scaledDy = dy / ellipse.Radius.Y;

        float closestX = Math.Clamp(0 + scaledDx, -scaledHalfWidth, scaledHalfWidth);
        float closestY = Math.Clamp(0 + scaledDy, -scaledHalfHeight, scaledHalfHeight);

        float distX = scaledDx - closestX;
        float distY = scaledDy - closestY;

        return (distX * distX + distY * distY) <= 1f;
    }
}