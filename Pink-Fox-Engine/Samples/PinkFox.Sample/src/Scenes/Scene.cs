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

    protected InputManager InputManager;
    protected AudioManager AudioManager;

    protected Texture2D PinkFoxLogoTexture;
    protected float PinkFoxLogoAngle = 0;

    private bool _Disposed;

    public Scene(Window window)
    {
        Window = window;
        InputManager = new();
        AudioManager = new(new MusicManager(), new SoundManager());

        PinkFoxLogoTexture = Texture2D.FromResource("PinkFoxIcon.png", Renderer);
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:
        Sound bonk = Sound.FromResource("Bonk.ogg");
        AudioManager.Sounds.RegisterSound("Bonk", bonk);
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:

        if (InputManager.Keyboard.IsKeyDown(SDL_Keycode.SDLK_Q))
        {
            // Should quit the program
            Window.Close();
        }

        if (InputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_LEFT))
        {
            PinkFoxLogoAngle -= 1f;
        }

        if (InputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_RIGHT))
        {
            PinkFoxLogoAngle += 1f;
        }

        if (InputManager.Keyboard.IsKeyDown(SDL_Keycode.SDLK_B))
        {
            AudioManager.Sounds.PlaySound("Bonk");
        }

        InputManager.Clear();
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

    public void OnSdlEventProcessed(SDL_Event sdlEvent)
    {
        InputManager.ProcessEvent(sdlEvent);
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