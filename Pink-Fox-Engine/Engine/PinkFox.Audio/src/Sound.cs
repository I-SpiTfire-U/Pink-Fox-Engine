using PinkFox.Core.Debugging;
using PinkFox.Core.Modules.Audio;
using SDL;

namespace PinkFox.Audio;

public class Sound : ISound, IDisposable
{
    public unsafe Mix_Chunk* Chunk { get; private set; }
    public float Volume { get; private set; } = 1f;
    private bool _Disposed;

    public static unsafe Sound FromResource(string resourceName)
    {
        Mix_Chunk* chunk = Core.ResourceManager.CreateChunkFromResource(resourceName);
        return new Sound(chunk);
    }

    public static unsafe Sound FromFile(string fileName)
    {
        Mix_Chunk* chunk = SDL3_mixer.Mix_LoadWAV(fileName);
        return new Sound(chunk);
    }

    public unsafe Sound(Mix_Chunk* chunk)
    {
        if (chunk is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to load sound effect");
            throw new Exception(SDL3.SDL_GetError());
        }

        Chunk = chunk;
    }

    public unsafe void Play(int numberOfLoops = 0)
    {
        int channel = SDL3_mixer.Mix_PlayChannel(-1, Chunk, numberOfLoops);
        if (channel == -1)
        {
            Terminal.LogMessage(LogLevel.Error, $"Failed to play sound: {SDL3.SDL_GetError()}");
        }
    }

    public void SetVolume(float volumeNormalized)
    {
        Volume = Math.Clamp(volumeNormalized, 0f, 1f);
        ApplyVolume();
    }

    public unsafe void ApplyVolume()
    {
        int sdlVolume = (int)(Volume * 128);
        _ = SDL3_mixer.Mix_VolumeChunk(Chunk, sdlVolume);
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
                Chunk = null;
            }
        }
        
        _Disposed = true;
    }
    
    ~Sound()
    {
        Dispose(false);
    }
}