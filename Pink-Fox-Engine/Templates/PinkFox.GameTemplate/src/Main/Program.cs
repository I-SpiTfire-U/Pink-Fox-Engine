using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.GameTemplate.Scenes;

namespace PinkFox.GameTemplate.Main;

public class Program
{
    public static void Main()
    {
        const int InitialWindowWidth = 800;
        const int InitialWindowHeight = 600;

        ResourceManager.LoadResources();

        using Engine engine = new();
        engine.InitializeWindowAndRenderer("My Game", "Icons_PinkFoxIcon", InitialWindowWidth, InitialWindowHeight);

        // engine.SetInputManager(new PinkFox.Input.InputManager());
        // engine.SetAudioManager(new PinkFox.Audio.AudioManager());
        // engine.SetVirtualRenderer(new PinkFox.Graphics.VirtualRenderer());

        engine.SetRenderClearColor(100, 149, 237);

        SceneManager.RegisterScene("Main Scene", new Scene(engine));
        SceneManager.PushScene("Main Scene");

        engine.Run();
    }
}