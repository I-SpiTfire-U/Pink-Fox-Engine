using System.Collections.ObjectModel;
using SDL3;

namespace PinkFox.Core.Interfaces;

public interface IGamepadCollection
{
    int Count { get; }
    bool AreGamepadsConnected { get; }
    ReadOnlyDictionary<uint, IGamepad> Connected { get; }

    void AddGamepad(uint instanceId, IGamepad gamepad);
    void RemoveGamepad(uint instanceId);
    void ProcessEvent(uint instanceId, SDL.Event sdlEvent);
    IGamepad? AtIndex(int index);
    IGamepad? GetFirstOrDefault();
    bool TryGetGamepad(uint instanceId, out IGamepad? gamepad);
    bool IsGamepadConnected(uint instanceId);
    public void UpdateGamepads();
    void Clear();
}