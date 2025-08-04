using PinkFox.Input.InputDevices;
using PinkFox.Core.Components;
using SDL;

namespace PinkFox.Input;

public class InputManager : IInputManager
{
    public IKeyboard Keyboard { get; } = new Keyboard();
    public IMouse Mouse { get; } = new Mouse();
    public IGamepadCollection Gamepads { get; } = new GamepadCollection();

    public void ProcessEvent(SDL_Event sdlEvent)
    {
        SDL_EventType eventType = sdlEvent.Type;

        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_KEY_DOWN or SDL_EventType.SDL_EVENT_KEY_UP:
                KeyboardEvent(eventType, sdlEvent);
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_MOTION or SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN or SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                MouseEvent(eventType, sdlEvent);
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED or SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED or SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN or SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP or SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
                GamepadEvent(eventType, sdlEvent);
                break;
        }
    }

    private void KeyboardEvent(SDL_EventType eventType, SDL_Event sdlEvent)
    {
        Keyboard.ProcessEvent(sdlEvent);
    }

    private void MouseEvent(SDL_EventType eventType, SDL_Event sdlEvent)
    {
        Mouse.ProcessEvent(sdlEvent);
    }

    private unsafe void GamepadEvent(SDL_EventType eventType, SDL_Event sdlEvent)
    {
        SDL_JoystickID instanceId = sdlEvent.gdevice.which;

        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_GAMEPAD_ADDED:
                if (!SDL3.SDL_IsGamepad(instanceId))
                {
                    return;
                }

                SDL_Gamepad* handle = SDL3.SDL_OpenGamepad(instanceId);
                if (handle is null)
                {
                    return;
                }

                Gamepad gamepad = new(handle);
                Gamepads.AddGamepad(instanceId, gamepad);
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_REMOVED:
                Gamepads.RemoveGamepad(instanceId);
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN or SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP or SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
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