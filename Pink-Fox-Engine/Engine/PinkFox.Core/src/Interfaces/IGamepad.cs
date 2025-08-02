using SDL3;

namespace PinkFox.Core.Interfaces;

public interface IGamepad
{
    nint Handle { get; }
    uint InstanceId { get; }

    bool IsButtonDown(SDL.GamepadButton button);
    bool IsButtonUp(SDL.GamepadButton button);
    bool IsButtonHeld(SDL.GamepadButton button);
    short GetAxis(SDL.GamepadAxis axis);

    float GetAxisFiltered(SDL.GamepadAxis axis);

    void Dispose();
    void ProcessEvent(SDL.Event e);
    void Clear();
}