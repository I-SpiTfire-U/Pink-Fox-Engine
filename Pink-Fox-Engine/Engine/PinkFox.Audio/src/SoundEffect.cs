using SDL;

namespace PinkFox.Audio;

public class SoundEffect : IDisposable
{
    public unsafe Mix_Chunk* Chunk { get; init; }
    private bool _Disposed;

    public unsafe SoundEffect(string resourceName)
    {
        Mix_Chunk* chunk = Core.ResourceManager.CreateChunkFromResource(resourceName);
        if (chunk is null)
        {
            throw new Exception("Failed to load sound effect");
        }

        Chunk = chunk;
    }

    public unsafe void Play(int numberOfLoops = 0)
    {
        int channel = SDL3_mixer.Mix_PlayChannel(-1, Chunk, numberOfLoops);
        if (channel == -1)
        {
            Console.WriteLine($"Failed to play sound: {SDL3.SDL_GetError()}");
        }
    }

    public void SetVolume(float volumeNormalized)
    {
        volumeNormalized = Math.Clamp(volumeNormalized, 0f, 1f);
        int sdlVolume = (int)(volumeNormalized * 128);
        unsafe
        {
            _ = SDL3_mixer.Mix_VolumeChunk(Chunk, sdlVolume);
        }
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
                SDL3_mixer.Mix_FreeChunk(Chunk);
            }
        }
        
        _Disposed = true;
    }
    
    ~SoundEffect()
    {
        Dispose(false);
    }
}