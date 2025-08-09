using PinkFox.Audio;
using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Rendering;
using PinkFox.Input;
using PongGame.Scenes;

namespace PinkFox.PongGame.Main;

public class Program
{
    public static unsafe void Main()
    {
        const int InitialWindowWidth = 612;
        const int InitialWindowHeight = 480;

        using Engine engine = new();
        engine.SetWindowFlags(SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        engine.InitializeWindowAndRenderer("PinkFox - Pong", @"Assets\Icon\Icon.png", InitialWindowWidth, InitialWindowHeight);

        engine.SetInputManager(new InputManager());
        engine.SetAudioManager(new AudioManager());
        engine.SetVirtualRenderer(new VirtualRenderer(engine.Renderer, InitialWindowWidth, InitialWindowHeight));

        engine.SetRenderClearColor(100, 149, 237);
        engine.VirtualRenderer.BorderColor = new() { r = 50, g = 99, b = 187, a = 255 };

        SceneManager.RegisterScene("Game Scene", new GameScene(engine));
        SceneManager.PushScene("Game Scene");

        engine.Run();
    }
}