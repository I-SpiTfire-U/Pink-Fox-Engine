using SDL;

namespace PinkFox.Core.Components;

public interface IGamepadCollection
{
    int Count { get; }
    bool AreGamepadsConnected { get; }

    bool IsGamepadConnected(SDL_JoystickID instanceId);
    void AddGamepad(SDL_JoystickID instanceId, IGamepad gamepad);
    void RemoveGamepad(SDL_JoystickID instanceId);
    bool TryGetGamepad(SDL_JoystickID instanceId, out IGamepad? gamepad);
    IGamepad? AtIndex(int index);
    IGamepad? GetFirstOrDefault();

    void ProcessEvent(SDL_JoystickID instanceId, SDL_Event sdlEvent);

    public void Update();
    void Clear();
}