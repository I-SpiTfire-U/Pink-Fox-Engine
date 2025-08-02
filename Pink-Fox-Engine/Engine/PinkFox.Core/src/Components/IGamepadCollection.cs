using SDL3;

namespace PinkFox.Core.Components;

public interface IGamepadCollection
{
    int Count { get; }
    bool AreGamepadsConnected { get; }

    bool IsGamepadConnected(uint instanceId);
    void AddGamepad(uint instanceId, IGamepad gamepad);
    void RemoveGamepad(uint instanceId);
    bool TryGetGamepad(uint instanceId, out IGamepad? gamepad);
    IGamepad? AtIndex(int index);
    IGamepad? GetFirstOrDefault();

    void ProcessEvent(uint instanceId, SDL.Event sdlEvent);

    public void Update();
    void Clear();
}