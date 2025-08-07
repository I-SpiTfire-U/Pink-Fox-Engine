using System.Numerics;

namespace PinkFox.Core.Collisions;

public readonly struct RectCollider : ICollider
{
    public Vector2 Position { get; init; }
    public Vector2 Size { get; init; }

    public Vector2 Center { get; init; }

    public float Left { get; init; }
    public float Right { get; init; }
    public float Top { get; init; }
    public float Bottom { get; init; }

    public RectCollider(float x, float y, float w, float h)
    {
        Size = new(w, h);
        Center = new(x + Size.X / 2, y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public RectCollider(float x, float y, Vector2 size)
    {
        Size = size;
        Center = new(x + Size.X / 2, y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public RectCollider(Vector2 position, float w, float h)
    {
        Size = new(w, h);
        Center = new(position.X + Size.X / 2, position.Y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public RectCollider(Vector2 position, Vector2 size)
    {
        Size = size;
        Center = new(position.X + Size.X / 2, position.Y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public bool IsCollidingWith(ICollider collider)
    {
        return collider switch
        {
            RectCollider rect => Collision.RectOnRectCollision(this, rect),
            CircleCollider circle => Collision.CircleOnRectCollision(circle, this),
            _ => false
        };
    }
}