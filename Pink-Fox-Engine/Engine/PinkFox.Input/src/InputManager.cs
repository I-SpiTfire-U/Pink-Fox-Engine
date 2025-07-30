using SDL3;
using PinkFox.Input.InputDevices;

namespace PinkFox.Input;

public static class InputManager
{
    public static Keyboard Keyboard { get; } = new();
    public static Mouse Mouse { get; } = new();

    private static readonly Dictionary<uint, Gamepad> _Gamepads = [];

    public static IReadOnlyDictionary<uint, Gamepad> Gamepads => _Gamepads;

    public static void ProcessEvent(SDL.Event e)
    {
        var eventType = (SDL.EventType)e.Type;

        switch (eventType)
        {
            case SDL.EventType.KeyDown or SDL.EventType.KeyUp:
                Keyboard.ProcessEvent(e);
                break;

            case SDL.EventType.MouseMotion or SDL.EventType.MouseButtonDown or SDL.EventType.MouseButtonUp:
                Mouse.ProcessEvent(e);
                break;

            case SDL.EventType.GamepadAdded:
                uint deviceIndex = e.GDevice.Which;
                if (!SDL.IsGamepad(deviceIndex))
                {
                    break;
                }
                nint handle = SDL.OpenGamepad(deviceIndex);
                if (handle != IntPtr.Zero)
                {
                    Gamepad gamepad = new(handle);
                    _Gamepads[gamepad.InstanceId] = gamepad;
                    Console.WriteLine($"Gamepad added: {SDL.GetGamepadName(handle)} (Instance ID: {gamepad.InstanceId})");
                }
                break;

            case SDL.EventType.GamepadRemoved:
                uint instanceId = e.GDevice.Which;
                if (_Gamepads.TryGetValue(instanceId, out Gamepad? removedPad))
                {
                    removedPad.Dispose();
                    _Gamepads.Remove(instanceId);
                    Console.WriteLine($"Removed gamepad: Instance ID {instanceId}");
                }
                break;

            case SDL.EventType.GamepadButtonDown or SDL.EventType.GamepadButtonUp or SDL.EventType.GamepadAxisMotion:
                if (_Gamepads.TryGetValue(e.GButton.Which, out Gamepad? pad))
                {
                    pad.ProcessEvent(e);
                }
                break;
        }
    }

    public static Gamepad? GetGamepadAtIndex(int index)
    {
        return (index < 0 || index >= Gamepads.Count) ? null : Gamepads.Values.ElementAt(index);
    }

    public static void Clear()
    {
        Keyboard.Clear();
        Mouse.Clear();
        foreach (var pad in _Gamepads.Values)
        {
            pad.Clear();
        }
    }
}