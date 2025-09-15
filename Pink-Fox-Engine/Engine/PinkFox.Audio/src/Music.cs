using PinkFox.Core.Debugging;
using PinkFox.Core.Modules.Audio;
using SDL;

namespace PinkFox.Audio;

public class Music : IMusic, IDisposable
{
    public unsafe Mix_Music* Track { get; private set; }
    public float Volume { get; private set; } = 1f;
    private bool _Disposed;

    public static unsafe Music FromResource(string resourceName)
    {
        Mix_Music* track = Core.ResourceManager.CreateMusicFromResource(resourceName);
        return new Music(track);
    }

    public static unsafe Music FromFile(string fileName)
    {
        Mix_Music* track = SDL3_mixer.Mix_LoadMUS(fileName);
        return new Music(track);
    }

    public unsafe Music(Mix_Music* track)
    {
        if (track is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to load music track");
            throw new Exception(SDL3.SDL_GetError());
        }

        Track = track;
    }

    public void SetVolume(float volumeNormalized)
    {
        Volume = Math.Clamp(volumeNormalized, 0f, 1f);
        ApplyVolume();
    }

    private unsafe void ApplyVolume()
    {
        int sdlVolume = (int)(Volume * 128);
        _ = SDL3_mixer.Mix_VolumeMusic(sdlVolume);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }

        if (disposing)
        {
            unsafe
            {
                SDL3_mixer.Mix_FreeMusic(Track);
                Track = null;
            }
        }

        _Disposed = true;
    }

    ~Music()
    {
        Dispose(false);
    }
}