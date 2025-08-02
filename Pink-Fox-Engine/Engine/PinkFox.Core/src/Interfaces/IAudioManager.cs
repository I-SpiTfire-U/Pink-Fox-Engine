namespace PinkFox.Core.Interfaces;

public interface IAudioManager
{
    bool IsPaused { get; set; }
    bool IsMusicPlaying { get; }

    void Init();
    void LoadSound(string id, string path);
    void PlaySound(string id, int loops = 0);
    void SetSoundVolume(string id, float volume);
    void UnloadSound(string id);

    void LoadTrack(string id, string path);
    void UnloadTrack(string id);
    void PlayMusic(string id, int numberOfLoops = 0);
    void PauseMusic();
    void ResumeMusic();
    void SetMusicVolume(float volumeNormalized);
    float GetMusicVolume();
    void Shutdown();
}