using System.Collections.Concurrent;
using PinkFox.Core.Components;
using SDL;

namespace PinkFox.Audio;

public class AudioManager : IAudioManager
{
    public bool IsPaused { get; set; } = false;
    public bool IsMusicPlaying => SDL3_mixer.Mix_PlayingMusic();

    private bool _Initialized = false;
    private MusicTrack? _CurrentMusic = null;
    
    private readonly ConcurrentDictionary<string, SoundEffect> _SoundEffects = new();
    private readonly ConcurrentDictionary<string, MusicTrack> _MusicTracks = new();

    public void Init()
    {
        if (_Initialized)
        {
            return;
        }

        uint flags = SDL3_mixer.MIX_INIT_MP3 | SDL3_mixer.MIX_INIT_OGG | SDL3_mixer.MIX_INIT_WAVPACK;
        uint result = SDL3_mixer.Mix_Init(flags);
        if ((result & SDL3_mixer.MIX_INIT_MP3) == 0)
        {
            Console.WriteLine($"Warning: MP3 format not supported.");
        }
        if ((result & SDL3_mixer.MIX_INIT_OGG) == 0)
        {
            Console.WriteLine("Warning: OGG format not supported.");
        }
        Console.WriteLine($"Mixer Init Result: {result}");

        unsafe
        {
            SDL_AudioSpec audioSpec = new()
            {
                freq = 44100,
                format = SDL_AudioFormat.SDL_AUDIO_S16LE,
                channels = 2
            };

            if (!SDL3_mixer.Mix_OpenAudio(SDL3.SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK, &audioSpec))
            {
                Console.WriteLine($"Mix_OpenAudio failed: {SDL3.SDL_GetError()}");
            }

            _ = SDL3_mixer.Mix_AllocateChannels(32);
        }

        _Initialized = true;
    }

    public void Shutdown()
    {
        TryEndMusic();

        foreach (var sfx in _SoundEffects.Values)
        {
            sfx.Dispose();
        }
        _SoundEffects.Clear();

        foreach (var track in _MusicTracks.Values)
        {
            track.Dispose();
        }
        _MusicTracks.Clear();

        SDL3_mixer.Mix_CloseAudio();
        SDL3_mixer.Mix_Quit();

        _Initialized = false;
    }

    public void LoadSound(string id, string path)
    {
        if (!_SoundEffects.ContainsKey(id))
        {
            _SoundEffects[id] = new SoundEffect(path);
        }
    }

    public void PlaySound(string id, int loops = 0)
    {
        if (_SoundEffects.TryGetValue(id, out SoundEffect? sfx))
        {
            sfx.Play(loops);
            return;
        }
        Console.WriteLine($"Sound '{id}' not loaded");
    }

    public void SetSoundVolume(string id, float volume)
    {
        if (_SoundEffects.TryGetValue(id, out SoundEffect? sfx))
        {
            sfx.SetVolume(volume);
        }
    }

    public void UnloadSound(string id)
    {
        if (_SoundEffects.TryRemove(id, out SoundEffect? sfx))
        {
            sfx.Dispose();
        }
    }

    public void LoadTrack(string id, string path)
    {
        if (!_MusicTracks.ContainsKey(id))
        {
            _MusicTracks[id] = new MusicTrack(path);
        }
    }

    public void UnloadTrack(string id)
    {
        if (_MusicTracks.TryRemove(id, out var track))
        {
            track.Dispose();
        }
    }

    public unsafe void PlayMusic(string id, int numberOfLoops = 0)
    {
        if (!_MusicTracks.TryGetValue(id, out MusicTrack? track))
        {
            Console.WriteLine($"Music Track '{id}' not loaded");
            return;
        }

        if (_CurrentMusic is not null && track.Track == _CurrentMusic.Track)
        {
            return;
        }

        TryEndMusic();
        _CurrentMusic = track;

        if (!SDL3_mixer.Mix_PlayMusic(_CurrentMusic.Track, numberOfLoops))
        {
            Console.WriteLine($"Failed to play music: {SDL3.SDL_GetError()}");
            _CurrentMusic = null;
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

    public void SetMusicVolume(float volume)
    {
        volume = Math.Clamp(volume, 0f, 1f);
        int sdlVolume = (int)(volume * 128);
        _ = SDL3_mixer.Mix_VolumeMusic(sdlVolume);
    }

    public unsafe float GetMusicVolume()
    {
        if (_CurrentMusic is null)
        {
            return 0f;
        }
        
        int sdlVolume = SDL3_mixer.Mix_VolumeMusic(-1);
        return sdlVolume / 128f;
    }

    private void TryEndMusic()
    {
        if (_CurrentMusic is not null)
        {
            SDL3_mixer.Mix_HaltMusic();
            _CurrentMusic = null;
        }
    }
}
