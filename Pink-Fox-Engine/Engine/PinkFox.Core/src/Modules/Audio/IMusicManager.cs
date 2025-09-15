namespace PinkFox.Core.Modules.Audio;

public interface IMusicManager : IDisposable
{
    public bool IsPaused { get; }
    public static bool IsMusicPlaying { get; }

    public void RegisterMusic(string id, IMusic music);
    public void UnregisterMusic(string id);

    public unsafe void PlayMusic(string id, int loops = 0, int fadeMs = 0);
    public void FadeOutMusic(int fadeMs);

    public void PauseMusic();
    public void ResumeMusic();

    public void SetCurrentTrackVolume(float volume);
    public unsafe float GetCurrentTrackVolume();
}