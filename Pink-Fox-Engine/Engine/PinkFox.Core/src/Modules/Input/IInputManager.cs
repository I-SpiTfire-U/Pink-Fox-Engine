using SDL;

namespace PinkFox.Core.Modules.Input;

public interface IInputManager : IDisposable
{
    IKeyboard Keyboard { get; }
    IMouse Mouse { get; }
    IGamepadCollection Gamepads { get; }

    void ProcessEvent(SDL_Event sdlEvent);

    void Clear();
}