using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.GameTemplate.Scenes;

namespace PinkFox.GameTemplate.Main;

public class Program
{
    [STAThread]
    public static void Main()
    {
        const int DefaultWindowWidth = 800;
        const int DefaultWindowHeight = 600;

        using Engine engine = new();

        // Enable for user input when using PinkFox.Input
        // engine.InputManager = new InputManager();

        // Enable for audio when using PinkFox.Audio
        // engine.AudioManager = new AudioManager();
        
        engine.OnStart = () =>
        {
            SceneManager.SetExitAction(engine.Stop);
            SceneManager.LoadScene(new SampleScene(engine.Renderer));
        };

        engine.Initialize("My Game", DefaultWindowWidth, DefaultWindowHeight);
        engine.Run();
    }
}