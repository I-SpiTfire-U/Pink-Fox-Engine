using System.Collections.Concurrent;
using PinkFox.Core.Debugging;
using PinkFox.Core.Modules.Audio;

namespace PinkFox.Audio;

public class SoundManager : ISoundManager, IDisposable
{
    private readonly ConcurrentDictionary<string, ISound> _SoundDictionary = new();
    private bool _Disposed = false;

    public void RegisterSound(string id, ISound sound)
    {
        if (_SoundDictionary.ContainsKey(id))
        {
            Terminal.LogMessage(LogLevel.Warning, $"Sound with the ID '{id}' already exists, skipping");
            return;
        }
        _SoundDictionary[id] = sound;
        Terminal.LogMessage(LogLevel.Success, $"Sound '{id}' registered");
    }

    public void UnregisterSound(string id)
    {
        if (!_SoundDictionary.TryRemove(id, out ISound? sound))
        {
            Terminal.LogMessage(LogLevel.Warning, $"No sound with the ID '{id}' is registered, skipping");
            return;
        }
        sound.Dispose();
        Terminal.LogMessage(LogLevel.Success, $"The sound '{id}' has been unregistered");
    }

    public void PlaySound(string id, int loops = 0)
    {
        if (_SoundDictionary.TryGetValue(id, out ISound? sfx))
        {
            sfx.Play(loops);
            Terminal.LogMessage(LogLevel.Success, $"Started playback of '{id}'");
            return;
        }
        Terminal.LogMessage(LogLevel.Warning, $"A sound effect with the ID '{id}' has not been loaded, playback cancelled");
    }

    public void SetSoundVolume(string id, float volume)
    {
        if (_SoundDictionary.TryGetValue(id, out ISound? sfx))
        {
            sfx.SetVolume(volume);
            Terminal.LogMessage(LogLevel.Success, $"Set sound volume of '{id}' to {volume}");
            return;
        }
        Terminal.LogMessage(LogLevel.Warning, $"A sound effect with the ID '{id}' has not been loaded, operation cancelled");
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
            foreach (Sound sfx in _SoundDictionary.Values)
            {
                sfx.Dispose();
            }
            _SoundDictionary.Clear();
        }

        _Disposed = true;
    }

    ~SoundManager()
    {
        Dispose(false);
    }
}