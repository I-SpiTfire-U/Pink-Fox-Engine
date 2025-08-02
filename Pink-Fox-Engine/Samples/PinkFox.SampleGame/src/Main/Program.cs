using PinkFox.Audio;
using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.Input;
using PinkFox.SampleGame.Scenes;

namespace PinkFox.SampleGame.Main;

public class Program
{
    public static void Main()
    {
        const int DefaultWindowWidth = 800;
        const int DefaultWindowHeight = 600;

        using Engine engine = new();
        engine.SetTargetFPS(120);

        // Enable for user input when using PinkFox.Input
        engine.InputManager = new InputManager();

        // Enable for audio when using PinkFox.Audio
        engine.AudioManager = new AudioManager();
        
        engine.OnStart = () =>
        {
            SceneManager.SetExitAction(engine.Stop);
            SceneManager.LoadScene(new SampleScene(engine.Renderer, DefaultWindowWidth, DefaultWindowHeight, engine.InputManager, engine.AudioManager));
        };

        engine.Initialize("My Game", DefaultWindowWidth, DefaultWindowHeight);
        engine.Run();
    }
}