using SDL;

namespace PinkFox.Core.Scenes;

public interface IScene : IDisposable
{
    public bool HasBeenLoaded { get; }

    public unsafe void LoadContent();

    public void Update(float deltaTime);
    public void FixedUpdate(float fixedUpdateInterval);
    public void Render(float deltaTime);
}