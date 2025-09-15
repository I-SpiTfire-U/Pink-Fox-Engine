using System.Runtime.CompilerServices;
using SDL;

namespace PinkFox.Core.Types;

public readonly struct FPoint(float x, float y)
{
    public float X { get; } = x;
    public float Y { get; } = y;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator SDL_FPoint(FPoint f)
        => new() { x = f.X, y = f.Y };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator FPoint(SDL_FPoint f)
        => new(f.x, f.y);

    public static readonly FPoint Empty = new(0, 0);
    public static readonly FPoint One = new(1, 1);
}