using SDL3;

namespace PinkFox.Core.Components;

public interface IInputManager
{
    IKeyboard Keyboard { get; }
    IMouse Mouse { get; }
    IGamepadCollection Gamepads { get; }

    void ProcessEvent(SDL.Event sdlEvent);
    void Clear();
}