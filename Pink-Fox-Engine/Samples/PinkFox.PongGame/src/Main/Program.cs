using PinkFox.Audio;
using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.Input;
using PongGame.Scenes;

namespace PinkFox.PongGame.Main;

public class Program
{
    public static void Main()
    {
        using Engine engine = new();
        engine.SetInputManager(new InputManager());
        engine.SetAudioManager(new AudioManager());

        engine.Initialize("PinkFox - Pong", @"Assets\Icon\Icon.png", 800, 600);
        engine.SetRenderDrawColor(0, 0, 0);
        // engine.EnableFPSLimit(true);
        // engine.SetTargetFPS(60);

        SceneManager.RegisterScene("Game Scene", new GameScene(engine));
        SceneManager.PushScene("Game Scene");

        engine.Run();
    }
}