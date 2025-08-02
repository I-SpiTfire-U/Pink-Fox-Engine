using PinkFox.Core.Components;
using SDL3;

namespace PinkFox.Input.InputDevices;

public class Keyboard : IKeyboard
{
    private readonly HashSet<SDL.Keycode> _keysDown = [];
    private readonly HashSet<SDL.Keycode> _keysUp = [];
    private readonly HashSet<SDL.Keycode> _keysHeld = [];

    public void ProcessEvent(SDL.Event e)
    {
        var eventType = (SDL.EventType)e.Type;
        if ((eventType == SDL.EventType.KeyDown) && e.Key.Repeat == false)
        {
            _keysDown.Add(e.Key.Key);
            _keysHeld.Add(e.Key.Key);
            Console.WriteLine($"Key down: {e.Key.Key}");
        }
        else if (eventType == SDL.EventType.KeyUp)
        {
            _keysUp.Add(e.Key.Key);
            _keysHeld.Remove(e.Key.Key);
            Console.WriteLine($"Key up: {e.Key.Key}");
        }
    }

    public bool IsKeyDown(SDL.Keycode key) => _keysDown.Contains(key);
    public bool IsKeyUp(SDL.Keycode key) => _keysUp.Contains(key);
    public bool IsKeyHeld(SDL.Keycode key) => _keysHeld.Contains(key);

    public void Clear()
    {
        _keysDown.Clear();
        _keysUp.Clear();
    }
}