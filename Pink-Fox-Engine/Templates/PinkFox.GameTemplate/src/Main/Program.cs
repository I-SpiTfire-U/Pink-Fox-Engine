using PinkFox.Core;
using PinkFox.Core.Types;
using PinkFox.GameTemplate.Scenes;

namespace PinkFox.GameTemplate.Main;

public class Program
{
    public static void Main()
    {
        const int WindowWidth = 1600;
        const int WindowHeight = 900;
        const string WindowTitle = "PinkFox Game Template";

        using Engine engine = new();
        engine.Initialize();

        ResourceManager.LoadResources();

        Window mainWindow = Window.Create(WindowWidth, WindowHeight, WindowTitle, "PinkFoxIcon.png", 0, null);
        mainWindow.Scenes.RegisterScene("MainScene", new Scene(mainWindow));
        mainWindow.Scenes.PushScene("MainScene");

        engine.AddWindow(mainWindow);

        engine.FPSIsLimited = true;
        engine.SetTargetFPS(60);
        engine.SetFixedUPS(60);

        engine.Run();
    }
}