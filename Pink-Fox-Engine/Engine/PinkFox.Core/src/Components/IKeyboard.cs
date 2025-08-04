using SDL;

namespace PinkFox.Core.Components;

public interface IKeyboard
{
    bool IsKeyDown(SDL_Keycode keycode);
    bool IsKeyUp(SDL_Keycode key);
    bool IsKeyHeld(SDL_Keycode key);

    void ProcessEvent(SDL_Event sdlEvent);
    
    void Clear();
}
