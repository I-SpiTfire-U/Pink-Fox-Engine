using System.Numerics;
using PinkFox.Core.Collisions;
using SDL;

namespace PinkFox.Core.Components;

public interface IMouse
{
    Vector2 Position { get; set; }

    bool IsButtonDown(SDL_MouseButtonFlags button);
    bool IsButtonUp(SDL_MouseButtonFlags button);
    bool IsButtonHeld(SDL_MouseButtonFlags button);

    void ProcessEvent(SDL_Event sdlEvent);
    
    void Clear();
}