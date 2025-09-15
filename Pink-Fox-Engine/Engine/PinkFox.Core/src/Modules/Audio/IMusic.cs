using SDL;

namespace PinkFox.Core.Modules.Audio;

public interface IMusic : IDisposable
{
    public unsafe Mix_Music* Track { get; }
    public float Volume { get; }

    public void SetVolume(float volumeNormalized);
}