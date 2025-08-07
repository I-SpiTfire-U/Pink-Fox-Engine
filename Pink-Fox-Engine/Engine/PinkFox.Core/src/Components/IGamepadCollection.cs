using SDL;

namespace PinkFox.Core.Components;

public interface IGamepadCollection
{
    int Count { get; }
    bool AreGamepadsConnected { get; }

    bool IsButtonDown(int gamepadIndex, SDL_GamepadButton button);
    bool IsButtonUp(int gamepadIndex, SDL_GamepadButton button);
    bool IsButtonHeld(int gamepadIndex, SDL_GamepadButton button);
    short GetAxis(int gamepadIndex, SDL_GamepadAxis axis);
    float GetAxisFiltered(int gamepadIndex, SDL_GamepadAxis axis);

    bool IsGamepadConnected(SDL_JoystickID instanceId);
    void AddGamepad(SDL_JoystickID instanceId, IGamepad gamepad);
    void RemoveGamepad(SDL_JoystickID instanceId);
    bool TryGetGamepad(SDL_JoystickID instanceId, out IGamepad? gamepad);

    void ProcessEvent(SDL_JoystickID instanceId, SDL_Event sdlEvent);

    public void Clear();
    void Dispose();
}