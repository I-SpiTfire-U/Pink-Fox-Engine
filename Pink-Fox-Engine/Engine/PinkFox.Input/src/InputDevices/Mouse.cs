using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Collisions;
using SDL;

namespace PinkFox.Input.InputDevices;

public class Mouse : IMouse
{
    private readonly HashSet<SDL_MouseButtonFlags> _ButtonsDown = [];
    private readonly HashSet<SDL_MouseButtonFlags> _ButtonsUp = [];
    private readonly HashSet<SDL_MouseButtonFlags> _ButtonsHeld = [];

    public Vector2 Position { get; set; }

    public void ProcessEvent(SDL_Event sdlEvent)
    {
        SDL_EventType eventType = sdlEvent.Type;

        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_MOUSE_MOTION:
                Position = new(sdlEvent.motion.x, sdlEvent.motion.y);
                Console.WriteLine($"Mouse position: {sdlEvent.motion.x}, {sdlEvent.motion.y}");
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_DOWN:
                _ButtonsDown.Add((SDL_MouseButtonFlags)sdlEvent.button.Button);
                _ButtonsHeld.Add((SDL_MouseButtonFlags)sdlEvent.button.Button);
                Console.WriteLine($"Mouse button down: {sdlEvent.button.Button}");
                break;

            case SDL_EventType.SDL_EVENT_MOUSE_BUTTON_UP:
                _ButtonsUp.Add((SDL_MouseButtonFlags)sdlEvent.button.Button);
                _ButtonsHeld.Remove((SDL_MouseButtonFlags)sdlEvent.button.Button);
                Console.WriteLine($"Mouse button up: {sdlEvent.button.Button}");
                break;
        }
    }

    public ICollider Collider => new RectCollider(Position, new(1, 1));

    public bool IsButtonDown(SDL_MouseButtonFlags button) => _ButtonsDown.Contains(button);
    public bool IsButtonUp(SDL_MouseButtonFlags button) => _ButtonsUp.Contains(button);
    public bool IsButtonHeld(SDL_MouseButtonFlags button) => _ButtonsHeld.Contains(button);

    public void Clear()
    {
        _ButtonsDown.Clear();
        _ButtonsUp.Clear();
    }
}