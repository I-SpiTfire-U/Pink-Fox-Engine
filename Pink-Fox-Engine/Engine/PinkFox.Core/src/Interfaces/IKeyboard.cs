using SDL3;

namespace PinkFox.Core.Interfaces;

public interface IKeyboard
{
    bool IsKeyDown(SDL.Keycode keycode);
    bool IsKeyUp(SDL.Keycode key);
    bool IsKeyHeld(SDL.Keycode key);

    void ProcessEvent(SDL.Event e);
    void Clear();
}
