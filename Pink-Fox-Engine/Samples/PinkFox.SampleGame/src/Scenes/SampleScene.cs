using PinkFox.Graphics.Basic;
using PinkFox.Core.Scenes;

namespace PinkFox.SampleGame.Scenes;

public class SampleScene : IScene
{
    private readonly Image2D _LexiImage;

    public SampleScene(nint renderer)
    {
        _LexiImage = new(0, 0, @"C:\Users\Lex\Downloads\518244405_2539279199798114_3918589501245530234_n.jpg", renderer, 0.5f);
    }

    public void LoadContent()
    {
        // Load game assets and content below:

    }

    public void Update(float deltaTime)
    {
        // Update game objects and code below:

    }

    public void FixedUpdate()
    {
        // Update physics code and fixed values here:

    }

    public void Draw(nint renderer)
    {
        // Draw graphics to the screen below:
        _LexiImage.Draw(renderer);
    }

    public event Action? OnRequestExit;
}