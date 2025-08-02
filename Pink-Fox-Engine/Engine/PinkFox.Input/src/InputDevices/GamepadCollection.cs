using PinkFox.Core.Components;
using SDL3;

namespace PinkFox.Input.InputDevices;

public class GamepadCollection : IGamepadCollection
{
    public int Count => _Gamepads.Count;
    public bool AreGamepadsConnected => _Gamepads.Count > 0;
    private readonly Dictionary<uint, IGamepad> _Gamepads = [];

    public void AddGamepad(uint instanceId, IGamepad gamepad)
    {
        _Gamepads.TryAdd(instanceId, gamepad);
        Console.WriteLine($"Gamepad added: {SDL.GetGamepadName(gamepad.Handle)} (Instance ID: {gamepad.InstanceId})");
    }

    public void RemoveGamepad(uint instanceId)
    {
        if (!IsGamepadConnected(instanceId))
        {
            return;
        }

        _Gamepads[instanceId].Dispose();
        _Gamepads.Remove(instanceId);
        Console.WriteLine($"Gamepad removed: Instance ID {instanceId}");
    }

    public void ProcessEvent(uint instanceId, SDL.Event sdlEvent)
    {
        if (!IsGamepadConnected(instanceId))
        {
            return;
        }

        _Gamepads[instanceId].ProcessEvent(sdlEvent);
        Console.WriteLine($"Processed Event: Instance ID {sdlEvent.GButton.Button}");
    }

    public IGamepad? AtIndex(int index)
    {
        return index >= 0 && index < _Gamepads.Count ? _Gamepads.ElementAt(index).Value : null;
    }

    public IGamepad? GetFirstOrDefault()
    {
        return AreGamepadsConnected ? _Gamepads.ElementAt(0).Value : null;
    }

    public bool TryGetGamepad(uint instanceId, out IGamepad? gamepad)
    {
        return _Gamepads.TryGetValue(instanceId, out gamepad);
    }

    public bool IsGamepadConnected(uint instanceId)
    {
        return _Gamepads.ContainsKey(instanceId);
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