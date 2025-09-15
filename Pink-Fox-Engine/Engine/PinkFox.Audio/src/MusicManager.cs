using System.Collections.Concurrent;
using PinkFox.Core.Debugging;
using PinkFox.Core.Modules.Audio;
using SDL;

namespace PinkFox.Audio;

public class MusicManager : IMusicManager, IDisposable
{
    public bool IsPaused { get; private set; } = false;
    public static bool IsMusicPlaying => SDL3_mixer.Mix_PlayingMusic();

    private IMusic? _CurrentMusic = null;
    private readonly ConcurrentDictionary<string, IMusic> _MusicDictionary = new();

    private bool _Disposed = false;

    public void RegisterMusic(string id, IMusic music)
    {
        if (_MusicDictionary.ContainsKey(id))
        {
            Terminal.LogMessage(LogLevel.Warning, $"Music with the ID '{id}' already exists, skipping");
            return;
        }
        _MusicDictionary[id] = music;
        Terminal.LogMessage(LogLevel.Success, $"Music '{id}' registered");
    }

    public void UnregisterMusic(string id)
    {
        if (!_MusicDictionary.TryRemove(id, out IMusic? music))
        {
            Terminal.LogMessage(LogLevel.Warning, $"No music with the ID '{id}' is registered, skipping");
            return;
        }
        music.Dispose();
        Terminal.LogMessage(LogLevel.Success, $"The music '{id}' has been unregistered");
    }

    public unsafe void PlayMusic(string id, int loops = 0, int fadeMs = 0)
    {
        if (!_MusicDictionary.TryGetValue(id, out IMusic? track))
        {
            Terminal.LogMessage(LogLevel.Warning, $"A music Track with the ID '{id}' has not been loaded, playback cancelled");
            return;
        }
        Terminal.LogMessage(LogLevel.Success, $"Found music track with the ID '{id}'");

        if (_CurrentMusic is not null && track.Track == _CurrentMusic.Track)
        {
            return;
        }

        if (_CurrentMusic is not null)
        {
            SDL3_mixer.Mix_HaltMusic();
            _CurrentMusic = null;
        }

        _CurrentMusic = track;

        if (fadeMs == 0 && !SDL3_mixer.Mix_PlayMusic(_CurrentMusic.Track, loops))
        {
            Terminal.LogMessage(LogLevel.Error, $"Failed to play music track '{id}'");
            throw new Exception(SDL3.SDL_GetError());
        }

        if (!SDL3_mixer.Mix_FadeInMusic(_CurrentMusic.Track, loops, fadeMs))
        {
            Terminal.LogMessage(LogLevel.Error, $"Failed to fade in music track '{id}'");
            throw new Exception(SDL3.SDL_GetError());
        }

        Terminal.LogMessage(LogLevel.Success, $"Started playback of '{id}'");
    }

    public void FadeOutMusic(int fadeMs)
    {
        if (!SDL3_mixer.Mix_FadeOutMusic(fadeMs))
        {
            Terminal.LogMessage(LogLevel.Error, $"Failed to fade out music");
            throw new Exception(SDL3.SDL_GetError());
        }
    }

    public void PauseMusic()
    {
        if (!IsPaused)
        {
            SDL3_mixer.Mix_PauseMusic();
            IsPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (IsPaused)
        {
            SDL3_mixer.Mix_ResumeMusic();
            IsPaused = false;
        }
    }

    public void SetCurrentTrackVolume(float volume)
    {
        _CurrentMusic?.SetVolume(volume);
    }

    public unsafe float GetCurrentTrackVolume()
    {
        return _CurrentMusic is not null ? _CurrentMusic.Volume * 128 : 0f;
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
            _CurrentMusic?.Dispose();
            _CurrentMusic = null;

            foreach (Music track in _MusicDictionary.Values)
            {
                track.Dispose();
            }
            _MusicDictionary.Clear();
        }

        _Disposed = true;
    }

    ~MusicManager()
    {
        Dispose(false);
    }
}