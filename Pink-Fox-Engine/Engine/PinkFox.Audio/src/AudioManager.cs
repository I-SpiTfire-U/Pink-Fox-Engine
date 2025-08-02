using System.Collections.Concurrent;
using PinkFox.Core.Interfaces;
using SDL3;

namespace PinkFox.Audio;

public class AudioManager : IAudioManager
{
    public bool IsPaused { get; set; } = false;
    private bool _Initialized = false;
    private nint _CurrentMusic = nint.Zero;
    private readonly ConcurrentDictionary<string, SoundEffect> _SoundEffects = new();
    private readonly ConcurrentDictionary<string, MusicTrack> _MusicTracks = new();

    public void Init()
    {
        if (_Initialized)
        {
            return;
        }

        Mixer.InitFlags flags = Mixer.InitFlags.MP3 | Mixer.InitFlags.OGG;
        Mixer.InitFlags result = Mixer.Init(flags);
        if ((result & flags) != flags)
        {
            Console.WriteLine($"Mixer.Init failed for some formats. SDL Error: {SDL.GetError()}");
        }

        Console.WriteLine($"Mixer Init Result: {result}");

        SDL.AudioSpec audioSpec = new()
        {
            Freq = 44100,
            Format = SDL.AudioFormat.AudioS16LE,
            Channels = 2
        };

        if (!Mixer.OpenAudio(SDL.AudioDeviceDefaultPlayback, audioSpec))
        {
            Console.WriteLine($"Mix_OpenAudio failed: {SDL.GetError()}");
            return;
        }

        _Initialized = true;
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
        if (_SoundEffects.TryGetValue(id, out var sfx))
        {
            sfx.Play(loops);
            return;
        }
        Console.WriteLine($"Sound '{id}' not loaded");
    }

    public void SetSoundVolume(string id, float volume)
    {
        if (_SoundEffects.TryGetValue(id, out var sfx))
        {
            sfx.SetVolume(volume);
        }
    }

    public void UnloadSound(string id)
    {
        if (_SoundEffects.TryRemove(id, out var sfx))
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

    public void PlayMusic(string id, int numberOfLoops = 0)
    {
        if (_MusicTracks.TryGetValue(id, out var track))
        {
            if (track.Track == _CurrentMusic)
            {
                return;
            }

            TryEndMusic();
            _CurrentMusic = track.Track;

            if (!Mixer.PlayMusic(_CurrentMusic, numberOfLoops))
            {
                Console.WriteLine($"Failed to play music: {SDL.GetError()}");
                _CurrentMusic = nint.Zero;
            }
            return;
        }
        Console.WriteLine($"Music Track '{id}' not loaded");
    }

    public void PauseMusic()
    {
        if (!IsPaused)
        {
            Mixer.PauseMusic();
            IsPaused = true;
        }
    }

    public void ResumeMusic()
    {
        if (IsPaused)
        {
            Mixer.ResumeMusic();
            IsPaused = false;
        }
    }

    public void SetMusicVolume(float volumeNormalized)
    {
        volumeNormalized = Math.Clamp(volumeNormalized, 0f, 1f);
        int sdlVolume = (int)(volumeNormalized * 128);
        _ = Mixer.VolumeMusic(sdlVolume);
    }

    public float GetMusicVolume()
    {
        int sdlVolume = Mixer.VolumeMusic(-1);
        return sdlVolume / 128f;
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

        Mixer.CloseAudio();
        Mixer.Quit();

        _Initialized = false;
    }

    private void TryEndMusic()
    {
        if (_CurrentMusic != nint.Zero)
        {
            Mixer.HaltMusic();
            _CurrentMusic = nint.Zero;
        }
    }
}
