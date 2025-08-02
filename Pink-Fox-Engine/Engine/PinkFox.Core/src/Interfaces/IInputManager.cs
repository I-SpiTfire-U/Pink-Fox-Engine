using SDL3;

namespace PinkFox.Core.Interfaces;

public interface IInputManager
{
    IKeyboard Keyboard { get; }
    IMouse Mouse { get; }
    IGamepadCollection Gamepads { get; }

    void ProcessEvent(SDL.Event sdlEvent);
    void Clear();
}