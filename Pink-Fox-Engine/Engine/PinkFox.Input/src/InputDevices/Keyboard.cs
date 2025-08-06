using PinkFox.Core.Components;
using SDL;

namespace PinkFox.Input.InputDevices;

public class Keyboard : IKeyboard
{
    private readonly HashSet<SDL_Keycode> _KeysDown = [];
    private readonly HashSet<SDL_Keycode> _KeysUp = [];
    private readonly HashSet<SDL_Keycode> _KeysHeld = [];

    public void ProcessEvent(SDL_Event sdlEvent)
    {
        SDL_EventType eventType = sdlEvent.Type;
        if ((eventType == SDL_EventType.SDL_EVENT_KEY_DOWN) && sdlEvent.key.repeat == false)
        {
            _KeysDown.Add(sdlEvent.key.key);
            _KeysHeld.Add(sdlEvent.key.key);
        }
        else if (eventType == SDL_EventType.SDL_EVENT_KEY_UP)
        {
            _KeysUp.Add(sdlEvent.key.key);
            _KeysHeld.Remove(sdlEvent.key.key);
        }
    }

    public bool IsKeyDown(SDL_Keycode key) => _KeysDown.Contains(key);
    public bool IsKeyUp(SDL_Keycode key) => _KeysUp.Contains(key);
    public bool IsKeyHeld(SDL_Keycode key) => _KeysHeld.Contains(key);

    public void Clear()
    {
        _KeysDown.Clear();
        _KeysUp.Clear();
    }
}