using SDL3;

namespace PinkFox.Audio;

public class SoundEffect : IDisposable
{
    public nint Chunk { get; init; }
    private bool _Disposed;

    public SoundEffect(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found", filePath);
        }

        Chunk = Mixer.LoadWAV(filePath);
        if (Chunk == nint.Zero)
        {
            throw new Exception("Failed to load sound effect");
        }
    }

    public void Play(int numberOfLoops = 0)
    {
        int channel = Mixer.PlayChannel(-1, Chunk, numberOfLoops);
        if (channel == -1)
        {
            Console.WriteLine($"Failed to play sound: {SDL.GetError()}");
        }
    }

    public void SetVolume(float volumeNormalized)
    {
        volumeNormalized = Math.Clamp(volumeNormalized, 0f, 1f);
        int sdlVolume = (int)(volumeNormalized * 128);
        _ = Mixer.VolumeChunk(Chunk, sdlVolume);
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
            Mixer.FreeChunk(Chunk);
        }
        _Disposed = true;
    }

    ~SoundEffect()
    {
        Dispose(false);
    }
}