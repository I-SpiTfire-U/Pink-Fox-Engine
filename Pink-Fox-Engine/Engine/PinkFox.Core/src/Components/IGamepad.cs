using SDL3;

namespace PinkFox.Core.Components;

public interface IGamepad
{
    nint Handle { get; }
    uint InstanceId { get; }

    bool IsButtonDown(SDL.GamepadButton button);
    bool IsButtonUp(SDL.GamepadButton button);
    bool IsButtonHeld(SDL.GamepadButton button);
    short GetAxis(SDL.GamepadAxis axis);
    float GetAxisFiltered(SDL.GamepadAxis axis);

    void ProcessEvent(SDL.Event e);

    void Clear();
    void Dispose();
}