using System.Numerics;
using SDL;

namespace PinkFox.Core.Modules.Input;

public interface IMouse
{
    bool AreAnyButtonsHeld { get; }
    Vector2 Position { get; set; }

    bool IsButtonDown(SDL_MouseButtonFlags button);
    bool IsButtonUp(SDL_MouseButtonFlags button);
    bool IsButtonHeld(SDL_MouseButtonFlags button);

    void ProcessEvent(SDL_Event sdlEvent);
    
    void Clear();
}