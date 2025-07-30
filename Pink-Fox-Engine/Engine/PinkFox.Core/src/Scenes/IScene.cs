namespace PinkFox.Core.Scenes;

public interface IScene
{
    public void LoadContent();
    public void Update(float deltaTime);
    public void FixedUpdate();
    public void Draw(nint renderer);

    event Action? OnRequestExit;
}