using SDL3;

namespace PinkFox.Input.InputDevices;

public class Gamepad
{
    public nint Handle { get; }
    public uint InstanceId { get; }

    private readonly HashSet<SDL.GamepadButton> _ButtonsDown = [];
    private readonly HashSet<SDL.GamepadButton> _ButtonsUp = [];
    private readonly HashSet<SDL.GamepadButton> _ButtonsHeld = [];

    private readonly Dictionary<SDL.GamepadAxis, short> _AxisValues = [];
    
    public Gamepad(nint handle)
    {
        Handle = handle;
        InstanceId = SDL.GetGamepadID(handle);
        string? name = SDL.GetGamepadName(handle);
        Console.WriteLine($"Opened gamepad: {name} (Instance ID: {InstanceId})");
    }

    public void ProcessEvent(SDL.Event e)
    {
        var eventType = (SDL.EventType)e.Type;

        switch (eventType)
        {
            case SDL.EventType.GamepadButtonDown:
                _ButtonsDown.Add((SDL.GamepadButton)e.GButton.Button);
                _ButtonsHeld.Add((SDL.GamepadButton)e.GButton.Button);
                break;

            case SDL.EventType.GamepadButtonUp:
                _ButtonsUp.Add((SDL.GamepadButton)e.GButton.Button);
                _ButtonsHeld.Remove((SDL.GamepadButton)e.GButton.Button);
                break;

            case SDL.EventType.GamepadAxisMotion:
                _AxisValues[(SDL.GamepadAxis)e.GAxis.Axis] = e.GAxis.Value;
                break;
        }
    }

    public bool IsButtonDown(SDL.GamepadButton button) => _ButtonsDown.Contains(button);
    public bool IsButtonUp(SDL.GamepadButton button) => _ButtonsUp.Contains(button);
    public bool IsButtonHeld(SDL.GamepadButton button) => _ButtonsHeld.Contains(button);

    public short GetAxis(SDL.GamepadAxis axis) => _AxisValues.TryGetValue(axis, out short value) ? value : (short)0;


    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            SDL.CloseGamepad(Handle);
        }
    }

    public void Clear()
    {
        _ButtonsDown.Clear();
        _ButtonsUp.Clear();
    }
}
