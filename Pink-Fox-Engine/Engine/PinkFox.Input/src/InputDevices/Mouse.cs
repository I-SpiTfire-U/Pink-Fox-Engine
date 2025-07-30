using SDL3;

namespace PinkFox.Input.InputDevices;

public class Mouse
{
    private readonly HashSet<SDL.MouseButtonFlags> _ButtonsDown = [];
    private readonly HashSet<SDL.MouseButtonFlags> _ButtonsUp = [];
    private readonly HashSet<SDL.MouseButtonFlags> _ButtonsHeld = [];

    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    public void ProcessEvent(SDL.Event e)
    {
        var eventType = (SDL.EventType)e.Type;

        switch (eventType)
        {
            case SDL.EventType.MouseMotion:
                MouseX = e.Motion.X;
                MouseY = e.Motion.Y;
                break;

            case SDL.EventType.MouseButtonDown:
                _ButtonsDown.Add((SDL.MouseButtonFlags)e.Button.Button);
                _ButtonsHeld.Add((SDL.MouseButtonFlags)e.Button.Button);
                break;

            case SDL.EventType.MouseButtonUp:
                _ButtonsUp.Add((SDL.MouseButtonFlags)e.Button.Button);
                _ButtonsHeld.Remove((SDL.MouseButtonFlags)e.Button.Button);
                break;
        }
    }

    public bool IsButtonDown(SDL.MouseButtonFlags button) => _ButtonsDown.Contains(button);
    public bool IsButtonUp(SDL.MouseButtonFlags button) => _ButtonsUp.Contains(button);
    public bool IsButtonHeld(SDL.MouseButtonFlags button) => _ButtonsHeld.Contains(button);

    public (float X, float Y) GetPosition() => (MouseX, MouseY);

    public void Clear()
    {
        _ButtonsDown.Clear();
        _ButtonsUp.Clear();
    }
}