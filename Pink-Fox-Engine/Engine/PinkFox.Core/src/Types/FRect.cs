using System.Runtime.CompilerServices;
using SDL;

namespace PinkFox.Core.Types;

public readonly struct FRect(float x, float y, float width, float height)
{
    public float X { get; } = x;
    public float Y { get; } = y;
    public float Width { get; } = width;
    public float Height { get; } = height;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SDL_FRect(FRect f)
        => new() { x = f.X, y = f.Y, w = f.Width, h = f.Height };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FRect(SDL_FRect f)
        => new(f.x, f.y, f.w, f.h);
    
    public static readonly FRect Empty = new(0, 0, 0, 0);
    public static readonly FRect Unit = new(0, 0, 1, 1);
}