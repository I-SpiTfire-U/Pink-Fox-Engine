using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.GameTemplate.Scenes;

namespace PinkFox.GameTemplate.Main;

public class Program
{
    public static void Main()
    {
        const int DefaultWindowWidth = 800;
        const int DefaultWindowHeight = 600;

        using Engine engine = new();
        engine.EnableFPSLimit(true);
        engine.SetTargetFPS(60);

        // Enable for user input when using PinkFox.Input
        // engine.SetInputManager(new InputManager());

        // Enable for audio when using PinkFox.Audio
        // engine.SetAudioManager(new AudioManager());
        
        // engine.SetWindowFlags();

        engine.OnStart = () =>
        {
            SceneManager.SetExitAction(engine.Stop);
            SceneManager.LoadScene(new SampleScene(engine));
        };

        engine.Initialize("My Game", @"Assets\Icon\Icon.png", DefaultWindowWidth, DefaultWindowHeight);
        engine.Run();
    }
}