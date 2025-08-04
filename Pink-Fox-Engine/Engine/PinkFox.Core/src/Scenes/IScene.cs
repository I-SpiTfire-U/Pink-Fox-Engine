namespace PinkFox.Core.Scenes;

public interface IScene
{
    event Action? OnRequestExit;

    void LoadContent();
    void Update(float deltaTime);
    void FixedUpdate(float fixedUpdateInterval);
    void Draw(nint renderer, float alpha);
    void OnWindowResize(int width, int height);
    
    void Dispose();
}