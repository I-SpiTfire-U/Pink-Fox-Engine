namespace PinkFox.Core.Scenes;

public interface IScene
{
    event Action? OnRequestExit;

    void LoadContent();
    void Update(float deltaTime);
    void FixedUpdate();
    void Draw(nint renderer);
    void OnWindowResize(int width, int height);
    
    void Dispose();
}