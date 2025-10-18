using PinkFox.Core.Modules.Audio;
using SDL;

namespace PinkFox.Audio;

public class AudioManager : IAudioManager
{
    public IMusicManager Music { get; init; }
    public ISoundManager Sounds { get; init; }

    public unsafe AudioManager(IMusicManager musicManager, ISoundManager soundManager)
    {
        Music = musicManager;
        Sounds = soundManager;
    }

    public void Initialize()
    {
        
    }

    public void Shutdown()
    {
        Music.Dispose();
        Sounds.Dispose();

        SDL3_mixer.Mix_CloseAudio();
        SDL3_mixer.Mix_Quit();
    }
}
