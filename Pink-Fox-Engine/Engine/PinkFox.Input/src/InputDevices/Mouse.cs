using System.Numerics;
using PinkFox.Core.Modules.Input;
using SDL;

namespace PinkFox.Input.InputDevices;

public class Mouse : IMouse
{
    public bool AreAnyButtonsHeld => _ButtonsHeld.Count > 0;
    public Vector2 Position { get; set; }
    
    private readonly HashSet<SDL_MouseButtonFlags> _ButtonsDown = [];
    private readonly HashSet<SDL_MouseButtonFlags> _ButtonsUp = [];
    private readonly HashSet<SDL_MouseButtonFlags> _ButtonsHeld = [];

    public void ProcessEvent(SDL_Event sdlEvent)
    {
        SDL_EventType eventType = sdlEvent.Type;

        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                Position = new(sdlEvent.motion.x, sdlEvent.motion.y);
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
                _ButtonsDown.Add((SDL_MouseButtonFlags)sdlEvent.button.Button);
                _ButtonsHeld.Add((SDL_MouseButtonFlags)sdlEvent.button.Button);
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                _ButtonsUp.Add((SDL_MouseButtonFlags)sdlEvent.button.Button);
                _ButtonsHeld.Remove((SDL_MouseButtonFlags)sdlEvent.button.Button);
                break;
        }
    }

    public bool IsButtonDown(SDL_MouseButtonFlags button) => _ButtonsDown.Contains(button);
    public bool IsButtonUp(SDL_MouseButtonFlags button) => _ButtonsUp.Contains(button);
    public bool IsButtonHeld(SDL_MouseButtonFlags button) => _ButtonsHeld.Contains(button);

    public void Clear()
    {
        _ButtonsDown.Clear();
        _ButtonsUp.Clear();
    }
}