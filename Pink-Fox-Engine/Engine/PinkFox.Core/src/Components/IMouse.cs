using System.Numerics;
using PinkFox.Core.Collisions;
using SDL3;

namespace PinkFox.Core.Components;

public interface IMouse
{
    Vector2 Position { get; set; }
    ICollider Collider { get; }

    bool IsButtonDown(SDL.MouseButtonFlags button);
    bool IsButtonUp(SDL.MouseButtonFlags button);
    bool IsButtonHeld(SDL.MouseButtonFlags button);

    void ProcessEvent(SDL.Event e);
    
    void Clear();
}