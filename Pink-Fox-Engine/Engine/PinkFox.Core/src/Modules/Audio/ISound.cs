using SDL;

namespace PinkFox.Core.Modules.Audio;

public interface ISound : IDisposable
{
    public unsafe Mix_Chunk* Chunk { get; }
    public float Volume { get; }

    public unsafe void Play(int loops = 0);

    public void SetVolume(float volumeNormalized);
}