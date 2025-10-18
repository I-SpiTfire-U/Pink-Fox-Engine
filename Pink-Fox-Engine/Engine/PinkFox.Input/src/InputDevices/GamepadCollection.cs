using PinkFox.Core.Modules.Input;
using SDL;

namespace PinkFox.Input.InputDevices;

public class GamepadCollection : IGamepadCollection, IDisposable
{
    public int Count => _Gamepads.Count;
    public bool AreGamepadsConnected => _Gamepads.Count > 0;
    private readonly List<SDL_JoystickID> _GamepadOrder = [];
    private readonly Dictionary<SDL_JoystickID, IGamepad> _Gamepads = [];

    private bool _Disposed;

    public bool AreAnyButtonsHeld(int gamepadIndex)
    {
        IGamepad? gamepad = AtIndex(gamepadIndex);
        if (gamepad is null)
        {
            //Console.WriteLine($"[Warning] Gamepad index '{gamepadIndex}' is out of range (Count: {Count}).");
            return false;
        }
        return gamepad.AreAnyButtonsHeld;
    }

    public bool IsButtonDown(int gamepadIndex, SDL_GamepadButton button)
    {
        IGamepad? gamepad = AtIndex(gamepadIndex);
        if (gamepad is null)
        {
            //Console.WriteLine($"[Warning] Gamepad index '{gamepadIndex}' is out of range (Count: {Count}).");
            return false;
        }
        return gamepad.IsButtonDown(button);
    }

    public bool IsButtonUp(int gamepadIndex, SDL_GamepadButton button)
    {
        IGamepad? gamepad = AtIndex(gamepadIndex);
        if (gamepad is null)
        {
            //Console.WriteLine($"[Warning] Gamepad index '{gamepadIndex}' is out of range (Count: {Count}).");
            return false;
        }
        return gamepad.IsButtonDown(button);
    }

    public bool IsButtonHeld(int gamepadIndex, SDL_GamepadButton button)
    {
        IGamepad? gamepad = AtIndex(gamepadIndex);
        if (gamepad is null)
        {
            //Console.WriteLine($"[Warning] Gamepad index '{gamepadIndex}' is out of range (Count: {Count}).");
            return false;
        }
        return gamepad.IsButtonDown(button);
    }

    public short GetAxis(int gamepadIndex, SDL_GamepadAxis axis)
    {
        IGamepad? controller = AtIndex(gamepadIndex);
        if (controller is null)
        {
            //Console.WriteLine($"[Warning] Gamepad index '{gamepadIndex}' is out of range (Count: {Count}).");
            return 0;
        }
        return controller.GetAxis(axis);
    }

    public float GetAxisFiltered(int gamepadIndex, SDL_GamepadAxis axis)
    {
        IGamepad? controller = AtIndex(gamepadIndex);
        if (controller is null)
        {
            //Console.WriteLine($"[Warning] Gamepad index '{gamepadIndex}' is out of range (Count: {Count}).");
            return 0;
        }
        return controller.GetAxisFiltered(axis);
    }

    public void AddGamepad(SDL_JoystickID instanceId, IGamepad gamepad)
    {
        if (_Gamepads.TryAdd(instanceId, gamepad))
        {
            _GamepadOrder.Add(instanceId);
        }
    }

    public void RemoveGamepad(SDL_JoystickID instanceId)
    {
        if (!IsGamepadConnected(instanceId))
        {
            return;
        }

        if (_Gamepads.Remove(instanceId, out IGamepad? gamepad))
        {
            gamepad.Dispose();
            _GamepadOrder.Remove(instanceId);
            Console.WriteLine($"Gamepad removed: Instance ID {instanceId}");
        }
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
    }

    public IGamepad? AtIndex(int index)
    {
        if (index < 0 || index >= _Gamepads.Count)
        {
            return null;
        }
        SDL_JoystickID instanceId = _GamepadOrder[index];
        return _Gamepads[instanceId];
    }

    public void Clear()
    {
        foreach (IGamepad gamepad in _Gamepads.Values)
        {
            gamepad.Clear();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }

        if (disposing)
        {
            Clear();
            foreach (IGamepad gamepad in _Gamepads.Values)
            {
                gamepad.Dispose();
            }
            _Gamepads.Clear();
        }
        _Disposed = true;
    }
    
    ~GamepadCollection()
    {
        Dispose(false);
    }
}