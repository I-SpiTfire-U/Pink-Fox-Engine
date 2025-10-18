using PinkFox.Audio;
using PinkFox.Core.Scenes;
using PinkFox.Core.Types;
using PinkFox.Graphics.Rendering;
using PinkFox.Input;
using SDL;

namespace PinkFox.Sample.Scenes;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Window Window;
    protected Renderer Renderer => Window.Renderer!;

    protected Texture2D PinkFoxLogoTexture;
    protected float PinkFoxLogoAngle = 0;

    private bool _Disposed;

    public Scene(Window window)
    {
        Window = window;

        PinkFoxLogoTexture = Texture2D.FromResource("PinkFoxIcon.png", Renderer);
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:
        
        Sound bonk = Sound.FromResource("Bonk.ogg");
        Window.GetAudioManager().Sounds.RegisterSound("Bonk", bonk);
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:

        if (Window.GetInputManager().Keyboard.IsKeyDown(SDL_Keycode.SDLK_Q))
        {
            Window.Close();
        }

        if (Window.GetInputManager().Keyboard.IsKeyHeld(SDL_Keycode.SDLK_LEFT))
        {
            PinkFoxLogoAngle -= 1f;
        }

        if (Window.GetInputManager().Keyboard.IsKeyHeld(SDL_Keycode.SDLK_RIGHT))
        {
            PinkFoxLogoAngle += 1f;
        }

        if (Window.GetInputManager().Keyboard.IsKeyDown(SDL_Keycode.SDLK_B))
        {
            Window.GetAudioManager().Sounds.PlaySound("Bonk");
        }

        Window.GetInputManager().Clear();
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:
        
    }

    public void Render(float deltaTime)
    {
        // TODO: Draw game elements to the screen below:

        PinkFoxLogoTexture.Draw(Renderer, new FRect(0, 0, PinkFoxLogoTexture.Width, PinkFoxLogoTexture.Height), new FRect(0, 0, PinkFoxLogoTexture.Width, PinkFoxLogoTexture.Height), PinkFoxLogoAngle);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }
        
        if (disposing) { }

        _Disposed = true;
    }
}