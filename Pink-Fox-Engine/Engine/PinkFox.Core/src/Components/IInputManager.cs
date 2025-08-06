using SDL;

namespace PinkFox.Core.Components;

public interface IInputManager
{
    IKeyboard Keyboard { get; }
    IMouse Mouse { get; }
    IGamepadCollection Gamepads { get; }

    void ProcessEvent(SDL_Event sdlEvent);

    void Clear();
    void Dispose();
}