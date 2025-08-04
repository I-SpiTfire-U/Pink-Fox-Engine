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
        engine.EnableFPSLimit(true);
        engine.SetTargetFPS(120);
        
        engine.SetInputManager(new InputManager());
        engine.SetAudioManager(new AudioManager());

        engine.OnStart = () =>
        {
            SceneManager.SetExitAction(engine.Stop);
            SceneManager.LoadScene(new SampleScene(engine));
        };

        engine.Initialize("Sample Game", @"Assets\Icon\Icon.png", DefaultWindowWidth, DefaultWindowHeight);
        engine.Run();
    }
}