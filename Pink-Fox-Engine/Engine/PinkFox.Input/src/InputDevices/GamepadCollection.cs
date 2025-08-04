using PinkFox.Core.Components;
using SDL;

namespace PinkFox.Input.InputDevices;

public class GamepadCollection : IGamepadCollection
{
    public int Count => _Gamepads.Count;
    public bool AreGamepadsConnected => _Gamepads.Count > 0;
    private readonly Dictionary<SDL_JoystickID, IGamepad> _Gamepads = [];

    public void AddGamepad(SDL_JoystickID instanceId, IGamepad gamepad)
    {
        _Gamepads.TryAdd(instanceId, gamepad);
    }

    public void RemoveGamepad(SDL_JoystickID instanceId)
    {
        if (!IsGamepadConnected(instanceId))
        {
            return;
        }

        _Gamepads[instanceId].Dispose();
        _Gamepads.Remove(instanceId);
        Console.WriteLine($"Gamepad removed: Instance ID {instanceId}");
    }

    public IGamepad? AtIndex(int index)
    {
        return index >= 0 && index < _Gamepads.Count ? _Gamepads.ElementAt(index).Value : null;
    }

    public IGamepad? GetFirstOrDefault()
    {
        return AreGamepadsConnected ? _Gamepads.ElementAt(0).Value : null;
    }

    public bool TryGetGamepad(SDL_JoystickID instanceId, out IGamepad? gamepad)
    {
        return _Gamepads.TryGetValue(instanceId, out gamepad);
    }

    public bool IsGamepadConnected(SDL_JoystickID instanceId)
    {
        return _Gamepads.ContainsKey(instanceId);
    }

    public void ProcessEvent(SDL_JoystickID instanceId, SDL_Event sdlEvent)
    {
        if (!IsGamepadConnected(instanceId))
        {
            return;
        }

        _Gamepads[instanceId].ProcessEvent(sdlEvent);
        Console.WriteLine($"Processed Event: Instance ID {sdlEvent.gbutton.Button}");
    }

    public void Update()
    {
        foreach (IGamepad gamepad in _Gamepads.Values)
        {
            gamepad.Clear();
        }
    }

    public void Clear()
    {
        foreach (IGamepad gamepad in _Gamepads.Values)
        {
            gamepad.Clear();
            gamepad.Dispose();
        }
        _Gamepads.Clear();
    }
}