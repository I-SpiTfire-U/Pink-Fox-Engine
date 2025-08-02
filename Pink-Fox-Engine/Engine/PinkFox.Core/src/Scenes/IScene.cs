using PinkFox.Core.Interfaces;

namespace PinkFox.Core.Scenes;

public interface IScene
{
    public void Update(float deltaTime);
    public void FixedUpdate();
    public void Draw(nint renderer);
    void OnWindowResize(int width, int height);

    event Action? OnRequestExit;
    public void Dispose();
}