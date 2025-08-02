using SDL3;
using PinkFox.Input.InputDevices;
using PinkFox.Core.Components;

namespace PinkFox.Input;

public class InputManager : IInputManager
{
    public IKeyboard Keyboard { get; } = new Keyboard();
    public IMouse Mouse { get; } = new Mouse();
    public IGamepadCollection Gamepads { get; } = new GamepadCollection();

    public void ProcessEvent(SDL.Event sdlEvent)
    {
        SDL.EventType eventType = (SDL.EventType)sdlEvent.Type;

        switch (eventType)
        {
            case SDL.EventType.KeyDown or SDL.EventType.KeyUp:
                KeyboardEvent(eventType, sdlEvent);
                break;

            case SDL.EventType.MouseMotion or SDL.EventType.MouseButtonDown or SDL.EventType.MouseButtonUp:
                MouseEvent(eventType, sdlEvent);
                break;

            case SDL.EventType.GamepadAdded or SDL.EventType.GamepadRemoved or SDL.EventType.GamepadButtonDown or SDL.EventType.GamepadButtonUp or SDL.EventType.GamepadAxisMotion:
                GamepadEvent(eventType, sdlEvent);
                break;
        }
    }

    private void KeyboardEvent(SDL.EventType eventType, SDL.Event sdlEvent)
    {
        Keyboard.ProcessEvent(sdlEvent);
    }

    private void MouseEvent(SDL.EventType eventType, SDL.Event sdlEvent)
    {
        Mouse.ProcessEvent(sdlEvent);
    }

    private void GamepadEvent(SDL.EventType eventType, SDL.Event sdlEvent)
    {
        uint instanceId = sdlEvent.GDevice.Which;

        switch (eventType)
        {
            case SDL.EventType.GamepadAdded:
                uint deviceIndex = sdlEvent.GDevice.Which;
                if (!SDL.IsGamepad(deviceIndex))
                {
                    return;
                }

                nint handle = SDL.OpenGamepad(deviceIndex);
                if (handle == nint.Zero)
                {
                    return;
                }

                Gamepad gamepad = new(handle);
                Gamepads.AddGamepad(instanceId, gamepad);
                break;

            case SDL.EventType.GamepadRemoved:
                Gamepads.RemoveGamepad(instanceId);
                break;

            case SDL.EventType.GamepadButtonDown or SDL.EventType.GamepadButtonUp or SDL.EventType.GamepadAxisMotion:
                Gamepads.ProcessEvent(instanceId, sdlEvent);
                break;
        }
    }

    public void Clear()
    {
        Keyboard.Clear();
        Mouse.Clear();
        Gamepads.Update();
    }
}