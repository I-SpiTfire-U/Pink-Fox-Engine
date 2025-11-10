using System.Numerics;
using PinkFox.Audio;
using PinkFox.Core.Scenes;
using PinkFox.Core.Types;
using PinkFox.Graphics;
using PinkFox.Graphics.Fonts;
using PinkFox.Graphics.Rendering;
using PinkFox.Input.InputDevices;
using SDL;

namespace PinkFox.Sample.Scenes;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Window Window;
    protected Renderer Renderer => Window.Renderer!;

    protected Sprite PinkFoxLogo;
    protected BoxSelect BoxSelect;
    protected Texture2D Shape;
    protected BitmapFont TestFont;
    protected Label TestLabel;

    private bool _Disposed;

    public Scene(Window window)
    {
        Window = window;

        PinkFoxLogo = new(Texture2D.FromResource("PinkFoxIcon.png", Renderer), Vector2.Zero, 0f);
        BoxSelect = new BoxSelect(new Texture2D(Renderer, (int)Window.Size.X, (int)Window.Size.Y), 5);
        Shape = TextureFactory.CreateCircleOutline(Renderer, 100, 100, 3, Color.White);

        TestFont = BitmapFont.FromFontResource("FortalesiaScript.font", Renderer);
        TestLabel = new(TestFont, "Hello, World!", 64, new Vector2(200, 100));
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

        TestLabel.Update(deltaTime);

        Keyboard keyboard = (Keyboard)Window.GetInputManager().Keyboard;

        PinkFoxLogo.Update(deltaTime, keyboard);

        if (keyboard.IsKeyHeld(SDL_Keycode.SDLK_Q))
        {
            Window.ExitProgram();
        }

        if (keyboard.IsKeyDown(SDL_Keycode.SDLK_B))
        {
            Window.GetAudioManager().Sounds.PlaySound("Bonk");
        }

        BoxSelect.Update(Renderer, Window.GetInputManager().Mouse);

        Window.GetInputManager().Clear();
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:
        
    }

    public void Render(float deltaTime)
    {
        // TODO: Draw game elements to the screen below:

        PinkFoxLogo.Render(Renderer);
        BoxSelect.Render(Renderer);

        Shape.Draw(Renderer, new FRect(0, 0, Shape.Width, Shape.Height), new FRect(100, 100, Shape.Width, Shape.Height));
        Shape.Draw(Renderer, new FRect(0, 0, Shape.Width, Shape.Height), new FRect(150, 150, Shape.Width, Shape.Height));
        TestLabel.Draw(Renderer);
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