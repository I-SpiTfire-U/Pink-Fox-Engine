using PinkFox.Core.Components;
using SDL;

namespace PinkFox.Input.InputDevices;

public class Gamepad : IGamepad, IDisposable
{
    public unsafe SDL_Gamepad* Handle { get; }
    public SDL_JoystickID InstanceId { get; }

    private readonly HashSet<SDL_GamepadButton> _ButtonsDown = [];
    private readonly HashSet<SDL_GamepadButton> _ButtonsUp = [];
    private readonly HashSet<SDL_GamepadButton> _ButtonsHeld = [];

    private readonly Dictionary<SDL_GamepadAxis, short> _AxisValues = [];
    private readonly Dictionary<SDL_GamepadAxis, short> _AxisDeadZones = new()
    {
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX, 8000 },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTY, 8000 },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTX, 8000 },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHTY, 8000 },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFT_TRIGGER, 3000 },
        { SDL_GamepadAxis.SDL_GAMEPAD_AXIS_RIGHT_TRIGGER, 3000 },
    };
    private bool _Disposed;

    private const short DeadZone = 8000;
    
    public unsafe Gamepad(SDL_Gamepad* handle)
    {
        Handle = handle;
        InstanceId = SDL3.SDL_GetGamepadID(handle);
        string? name = SDL3.SDL_GetGamepadName(handle);
        Console.WriteLine($"Opened gamepad: {name} (Instance ID: {InstanceId})");
    }

    public void ProcessEvent(SDL_Event sdlEvent)
    {
        SDL_EventType eventType = sdlEvent.Type;

        switch (eventType)
        {
            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_DOWN:
                _ButtonsDown.Add(sdlEvent.gbutton.Button);
                _ButtonsHeld.Add(sdlEvent.gbutton.Button);
                Console.WriteLine($"Button down: {sdlEvent.gbutton.Button}");
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_BUTTON_UP:
                _ButtonsUp.Add(sdlEvent.gbutton.Button);
                _ButtonsHeld.Remove(sdlEvent.gbutton.Button);
                Console.WriteLine($"Button up: {sdlEvent.gbutton.Button}");
                break;

            case SDL_EventType.SDL_EVENT_GAMEPAD_AXIS_MOTION:
                _AxisValues[sdlEvent.gaxis.Axis] = sdlEvent.gaxis.value;
                Console.WriteLine($"Axis {sdlEvent.gaxis.Axis}: {sdlEvent.gaxis.value}");
                break;
        }
    }

    public bool IsButtonDown(SDL_GamepadButton button) => _ButtonsDown.Contains(button);
    public bool IsButtonUp(SDL_GamepadButton button) => _ButtonsUp.Contains(button);
    public bool IsButtonHeld(SDL_GamepadButton button) => _ButtonsHeld.Contains(button);
    public short GetAxis(SDL_GamepadAxis axis) => _AxisValues.TryGetValue(axis, out short value) ? value : (short)0;

    public float GetAxisFiltered(SDL_GamepadAxis axis)
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
            unsafe
            {
                if (Handle is not null)
                {
                    Clear();
                    SDL3.SDL_CloseGamepad(Handle);
                }
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
