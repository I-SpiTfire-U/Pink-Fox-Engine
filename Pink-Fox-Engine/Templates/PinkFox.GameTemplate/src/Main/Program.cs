using PinkFox.Core;
using PinkFox.Core.Types;
using PinkFox.GameTemplate.Scenes;

namespace PinkFox.GameTemplate.Main;

public class Program
{
    public static void Main()
    {
        ResourceManager.LoadResources();

        const int WindowWidth = 1600;
        const int WindowHeight = 900;

        Window mainWindow = Window.Create(WindowWidth, WindowHeight, "PinkFox Test", "PinkFoxIcon.png", 0, null);
        mainWindow.Scenes.RegisterScene("MainScene", new Scene(mainWindow));
        mainWindow.Scenes.PushScene("MainScene");

        using Engine engine = new();
        engine.Initialize();

        engine.AddWindow(mainWindow);

        engine.FPSIsLimited = true;
        engine.SetTargetFPS(60);
        engine.SetFixedUPS(60);

        engine.Run();
    }
}