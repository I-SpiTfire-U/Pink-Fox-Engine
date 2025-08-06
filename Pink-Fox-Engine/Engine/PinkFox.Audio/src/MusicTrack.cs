using SDL;

namespace PinkFox.Audio;

public class MusicTrack : IDisposable
{
    public unsafe Mix_Music* Track { get; init; }
    private bool _Disposed;

    public unsafe MusicTrack(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found", filePath);
        }

        Mix_Music* track = SDL3_mixer.Mix_LoadMUS(filePath);
        if (track is null)
        {
            throw new Exception($"Failed to load music track: {SDL3.SDL_GetError()}");
        }

        Track = track;
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
            }
        }

        _Disposed = true;
    }

    ~MusicTrack()
    {
        Dispose(false);
    }
}