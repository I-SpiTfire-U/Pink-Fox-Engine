using SDL3;

namespace PinkFox.Core.Components;

public interface IKeyboard
{
    bool IsKeyDown(SDL.Keycode keycode);
    bool IsKeyUp(SDL.Keycode key);
    bool IsKeyHeld(SDL.Keycode key);

    void ProcessEvent(SDL.Event e);
    void Clear();
}
