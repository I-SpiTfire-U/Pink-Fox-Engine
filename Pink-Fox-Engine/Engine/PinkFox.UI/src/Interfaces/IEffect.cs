using System.Numerics;

namespace PinkFox.UI.Interfaces;

public interface IEffect
{
    public bool Looping { get; }
    public bool IsRunning { get; }
    public float Timer { get; }
    public float MaxDuration { get; }

    public void Initialize();
    public void Update(float deltaTime);
    Vector2 ApplyEffectToCharacter(char c, int index, Vector2 basePosition, float scale);
}