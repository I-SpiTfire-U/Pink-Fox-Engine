using System.Numerics;
using PinkFox.UI.Interfaces;

namespace PinkFox.UI.Effects;

public class ShakeEffect : IEffect
{
    public bool Looping { get; private set; } = false;
    public bool IsRunning { get; private set; } = false;
    public float Timer { get; private set; } = 0f;
    public Vector2 Intensity { get; private set; } = Vector2.Zero;

    public float MaxDuration { get; init; }
    public Vector2 DefaultIntensity { get; init; }

    public ShakeEffect(float maxDuration, Vector2 defaultIntensity, bool looping, bool startImmediately = false)
    {
        Looping = looping;
        MaxDuration = maxDuration;
        DefaultIntensity = defaultIntensity;

        if (startImmediately)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        IsRunning = true;
        Timer = MaxDuration;
        Intensity = DefaultIntensity;
    }

    public void Update(float deltaTime)
    {
        if (!IsRunning)
        {
            Intensity = Vector2.Zero;
            return;
        }

        Timer -= deltaTime;
        if (Timer <= 0f)
        {
            IsRunning = false;
        }

        if (Looping)
        {
            Initialize();
        }
    }

    public Vector2 ApplyEffectToCharacter(char c, int index, Vector2 basePosition, float scale)
    {
        if (!IsRunning)
        {
            return Vector2.Zero;
        }

        float offsetX = (float)(Random.Shared.NextDouble() * 2 - 1) * Intensity.X * scale;
        float offsetY = (float)(Random.Shared.NextDouble() * 2 - 1) * Intensity.Y * scale;

        return new Vector2(offsetX, offsetY);
    }
}