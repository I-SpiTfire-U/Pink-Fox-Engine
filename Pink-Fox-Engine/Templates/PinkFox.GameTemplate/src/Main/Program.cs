using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.GameTemplate.Scenes;

namespace PinkFox.GameTemplate.Main;

public class Program
{
    public static void Main()
    {
        using Engine engine = new();
        // engine.SetInputManager(new PinkFox.Input.InputManager());
        // engine.SetAudioManager(new PinkFox.Audio.AudioManager());

        engine.Initialize("My Game", @"Assets\Icon\Icon.png", 800, 600);
        engine.SetRenderDrawColor(100, 149, 237);

        SceneManager.RegisterScene("Main Scene", new Scene(engine));
        SceneManager.PushScene("Main Scene");

        engine.Run();
    }
}