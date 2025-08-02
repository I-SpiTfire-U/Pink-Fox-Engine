using PinkFox.Core.Components;
using SDL3;

namespace PinkFox.Input.InputDevices;

public class Gamepad : IGamepad, IDisposable
{
    public nint Handle { get; }
    public uint InstanceId { get; }

    private readonly HashSet<SDL.GamepadButton> _ButtonsDown = [];
    private readonly HashSet<SDL.GamepadButton> _ButtonsUp = [];
    private readonly HashSet<SDL.GamepadButton> _ButtonsHeld = [];

    private readonly Dictionary<SDL.GamepadAxis, short> _AxisValues = [];
    private readonly Dictionary<SDL.GamepadAxis, short> _AxisDeadZones = new()
    {
        { SDL.GamepadAxis.LeftX, 8000 },
        { SDL.GamepadAxis.LeftY, 8000 },
        { SDL.GamepadAxis.RightX, 8000 },
        { SDL.GamepadAxis.RightY, 8000 },
        { SDL.GamepadAxis.LeftTrigger, 3000 },
        { SDL.GamepadAxis.RightTrigger, 3000 },
    };
    private bool _Disposed;

    private const short DeadZone = 8000;
    
    public Gamepad(nint handle)
    {
        Handle = handle;
        InstanceId = SDL.GetGamepadID(handle);
        string? name = SDL.GetGamepadName(handle);
        Console.WriteLine($"Opened gamepad: {name} (Instance ID: {InstanceId}) (Handle: {handle})");
    }

    public void ProcessEvent(SDL.Event e)
    {
        SDL.EventType eventType = (SDL.EventType)e.Type;

        switch (eventType)
        {
            case SDL.EventType.GamepadButtonDown:
                _ButtonsDown.Add((SDL.GamepadButton)e.GButton.Button);
                _ButtonsHeld.Add((SDL.GamepadButton)e.GButton.Button);
                Console.WriteLine($"Button down: {e.GButton.Button}");
                break;

            case SDL.EventType.GamepadButtonUp:
                _ButtonsUp.Add((SDL.GamepadButton)e.GButton.Button);
                _ButtonsHeld.Remove((SDL.GamepadButton)e.GButton.Button);
                Console.WriteLine($"Button up: {e.GButton.Button}");
                break;

            case SDL.EventType.GamepadAxisMotion:
                _AxisValues[(SDL.GamepadAxis)e.GAxis.Axis] = e.GAxis.Value;
                Console.WriteLine($"Axis {e.GAxis.Axis}: {e.GAxis.Value}");
                break;
        }
    }

    public bool IsButtonDown(SDL.GamepadButton button) => _ButtonsDown.Contains(button);
    public bool IsButtonUp(SDL.GamepadButton button) => _ButtonsUp.Contains(button);
    public bool IsButtonHeld(SDL.GamepadButton button) => _ButtonsHeld.Contains(button);
    public short GetAxis(SDL.GamepadAxis axis) => _AxisValues.TryGetValue(axis, out short value) ? value : (short)0;

    public float GetAxisFiltered(SDL.GamepadAxis axis)
    {
        if (!_AxisValues.TryGetValue(axis, out short rawValue))
        {
            return 0f;
        }

        short dz = _AxisDeadZones.TryGetValue(axis, out var val) ? val : DeadZone;
        if ((rawValue > -dz) && (rawValue < dz))
        {
            return 0f;
        }

        return Math.Clamp(rawValue / 32767f, -1f, 1f);
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
            if (Handle != nint.Zero)
            {
                Clear();
                SDL.CloseGamepad(Handle);
            }
        }
        _Disposed = true;
    }

    public void Clear()
    {
        _ButtonsDown.Clear();
        _ButtonsUp.Clear();
    }
}
