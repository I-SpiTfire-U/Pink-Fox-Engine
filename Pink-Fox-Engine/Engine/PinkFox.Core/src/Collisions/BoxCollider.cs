using System.Numerics;
using SDL;

namespace PinkFox.Core.Collisions;

public readonly struct BoxCollider : ICollider
{
    public Vector2 Position { get; init; }
    public Vector2 Size { get; init; }

    public Vector2 Center { get; init; }

    public float Left { get; init; }
    public float Right { get; init; }
    public float Top { get; init; }
    public float Bottom { get; init; }

    public BoxCollider(float x, float y, float w, float h)
    {
        Size = new(w, h);
        Center = new(x + Size.X / 2, y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public BoxCollider(float x, float y, Vector2 size)
    {
        Size = size;
        Center = new(x + Size.X / 2, y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public BoxCollider(Vector2 position, float w, float h)
    {
        Size = new(w, h);
        Center = new(position.X + Size.X / 2, position.Y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public BoxCollider(Vector2 position, Vector2 size)
    {
        Size = size;
        Center = new(position.X + Size.X / 2, position.Y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public BoxCollider(SDL_Rect fRect)
    {
        Size = new(fRect.w, fRect.h);
        Center = new(fRect.x + Size.X / 2, fRect.y + Size.Y / 2);

        Left = Center.X - Size.X / 2;
        Right = Center.X + Size.X / 2;
        Top = Center.Y - Size.Y / 2;
        Bottom = Center.Y + Size.Y / 2;

        Position = new(Left, Top);
    }

    public BoxCollider(SDL_FRect fRect)
    {
        Size = new(fRect.w, fRect.h);
        Center = new(fRect.x + Size.X / 2, fRect.y + Size.Y / 2);

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
            BoxCollider rect => Collision.BoxOnBoxCollision(this, rect),
            CircleCollider circle => Collision.CircleOnBoxCollision(circle, this),
            _ => false
        };
    }
}