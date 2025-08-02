using SDL3;

namespace PinkFox.Audio;

public class MusicTrack : IDisposable
{
    public nint Track { get; init; }
    private bool _Disposed;

    public MusicTrack(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found", filePath);
        }

        Track = Mixer.LoadMUS(filePath);
        if (Track == nint.Zero)
        {
            throw new Exception("Failed to load sound effect");
        }
    }

    #region Disposal
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
            Mixer.FreeMusic(Track);
        }
        _Disposed = true;
    }

    ~MusicTrack()
    {
        Dispose(false);
    }
    #endregion
}