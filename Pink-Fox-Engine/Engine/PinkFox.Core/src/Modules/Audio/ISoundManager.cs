namespace PinkFox.Core.Modules.Audio;

public interface ISoundManager : IDisposable
{
    public void RegisterSound(string id, ISound sound);
    public void UnregisterSound(string id);

    public void PlaySound(string id, int loops = 0);

    public void SetSoundVolume(string id, float volume);
}