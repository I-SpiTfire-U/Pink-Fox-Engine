using PinkFox.Audio;
using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.Input;
using PinkFox.Sample2.Scenes;

namespace PinkFox.Sample2.Main;

public class Program
{
    public static void Main()
    {
        using Engine engine = new();
        engine.InitializeWindowAndRenderer("My Game", @"Assets\Icon\Icon.png", 800, 600);
        engine.SetRenderClearColor(0, 0, 0);

        engine.SetInputManager(new InputManager());
        engine.SetAudioManager(new AudioManager());
        // engine.SetVirtualRenderer(new PinkFox.Graphics.VirtualRenderer());

        SceneManager.RegisterScene("Main Scene", new Scene(engine));
        SceneManager.PushScene("Main Scene");

        engine.Run();
    }
}