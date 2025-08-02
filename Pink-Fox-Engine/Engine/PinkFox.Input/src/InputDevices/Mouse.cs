using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Collisions;
using SDL3;

namespace PinkFox.Input.InputDevices;

public class Mouse : IMouse
{
    private readonly HashSet<SDL.MouseButtonFlags> _ButtonsDown = [];
    private readonly HashSet<SDL.MouseButtonFlags> _ButtonsUp = [];
    private readonly HashSet<SDL.MouseButtonFlags> _ButtonsHeld = [];

    public Vector2 Position { get; set; }

    public void ProcessEvent(SDL.Event e)
    {
        var eventType = (SDL.EventType)e.Type;

        switch (eventType)
        {
            case SDL.EventType.MouseMotion:
                Position = new(e.Motion.X, e.Motion.Y);
                Console.WriteLine($"Mouse position: {e.Motion.X}, {e.Motion.Y}");
                break;

            case SDL.EventType.MouseButtonDown:
                _ButtonsDown.Add((SDL.MouseButtonFlags)e.Button.Button);
                _ButtonsHeld.Add((SDL.MouseButtonFlags)e.Button.Button);
                Console.WriteLine($"Mouse button down: {e.Button.Button}");
                break;

            case SDL.EventType.MouseButtonUp:
                _ButtonsUp.Add((SDL.MouseButtonFlags)e.Button.Button);
                _ButtonsHeld.Remove((SDL.MouseButtonFlags)e.Button.Button);
                Console.WriteLine($"Mouse button up: {e.Button.Button}");
                break;
        }
    }

    public ICollider Collider => new RectCollider(Position.X, Position.Y, 1, 1);

    public bool IsButtonDown(SDL.MouseButtonFlags button) => _ButtonsDown.Contains(button);
    public bool IsButtonUp(SDL.MouseButtonFlags button) => _ButtonsUp.Contains(button);
    public bool IsButtonHeld(SDL.MouseButtonFlags button) => _ButtonsHeld.Contains(button);

    public void Clear()
    {
        _ButtonsDown.Clear();
        _ButtonsUp.Clear();
    }
}