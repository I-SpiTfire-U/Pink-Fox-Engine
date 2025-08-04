namespace PinkFox.SampleGame;

public class Velocity(float accelerationRate, float maximumVelocity, float currentVelocity = 0f)
{
    public float AccelerationRate { get; init; } = accelerationRate;
    public float MaximumVelocity { get; init; } = maximumVelocity;
    public float CurrentVelocity { get; set; } = currentVelocity;
}