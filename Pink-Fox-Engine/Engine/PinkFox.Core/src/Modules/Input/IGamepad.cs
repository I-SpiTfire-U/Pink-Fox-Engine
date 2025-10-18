using SDL;

namespace PinkFox.Core.Modules.Input;

public interface IGamepad
{
    unsafe SDL_Gamepad* Handle { get; }
    bool AreAnyButtonsHeld { get; }
    SDL_JoystickID InstanceId { get; }

    bool IsButtonDown(SDL_GamepadButton button);
    bool IsButtonUp(SDL_GamepadButton button);
    bool IsButtonHeld(SDL_GamepadButton button);
    short GetAxis(SDL_GamepadAxis axis);
    float GetAxisFiltered(SDL_GamepadAxis axis);

    void ProcessEvent(SDL_Event sdlEvent);

    void Clear();
    void Dispose();
}