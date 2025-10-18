namespace PinkFox.Core.Modules.Audio;

public interface IAudioManager
{
    public IMusicManager Music { get; init; }
    public ISoundManager Sounds { get; init; }
    void Shutdown();
}