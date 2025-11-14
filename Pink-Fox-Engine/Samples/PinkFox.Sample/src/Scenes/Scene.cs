using System.Numerics;
using PinkFox.Audio;
using PinkFox.Core.Collisions;
using PinkFox.Core.Scenes;
using PinkFox.Core.Types;
using PinkFox.Graphics;
using PinkFox.Graphics.Rendering;
using PinkFox.Input.InputDevices;
using PinkFox.UI.Fonts;
using PinkFox.UI.Types;
using SDL;

namespace PinkFox.Sample.Scenes;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Window Window;
    protected Renderer Renderer => Window.Renderer!;

    protected Sprite PinkFoxLogo;
    protected Sprite DvdLogo;
    protected BoxSelect BoxSelect;
    protected Texture2D Shape;
    protected BitmapFont TestFont;
    protected StaticLabel TestLabel;
    protected Button BasicButton;

    private bool _Disposed;

    public Scene(Window window)
    {
        Window = window;

        PinkFoxLogo = new(Texture2D.FromResource("PinkFoxIcon.png", Renderer), Vector2.Zero, 0f);
        DvdLogo = new(Texture2D.FromResource("dvd-logo.png", Renderer), new Vector2(-100, -600), 0f);
        
        BoxSelect = new BoxSelect(new Texture2D(Renderer, (int)Window.Size.X, (int)Window.Size.Y), 5);
        Shape = TextureFactory.CreateCircleOutline(Renderer, 100, 100, 3, Color.White);

        BasicButton = new(new RectCollider2D(500, 500, 150, 30), Renderer);
        BasicButton.IsClicked += () => { Window.GetAudioManager().Sounds.PlaySound("Bonk"); };

        TestFont = BitmapFont.FromFontResource("LTInternet-Regular.font", Renderer);
        TestLabel = new(TestFont, "> Bonk! <", 32, new Vector2(520, 500));
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:

        Sound bonk = Sound.FromResource("Bonk.ogg");
        Music Asgore = Music.FromResource("Asgore.mp3");
        Window.GetAudioManager().Sounds.RegisterSound("Bonk", bonk);
        Window.GetAudioManager().Music.RegisterMusic("Asgore", Asgore);

        Window.GetAudioManager().Music.PlayMusic("Asgore", -1);
    } 

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:

        Window.SetRenderClearColor((byte)PinkFoxLogo.Position.X, (byte)PinkFoxLogo.Position.Y, (byte)PinkFoxLogo.Angle);

        TestLabel.Update(deltaTime);

        Keyboard keyboard = (Keyboard)Window.GetInputManager().Keyboard;

        PinkFoxLogo.Update(deltaTime, keyboard);

        if (keyboard.IsKeyHeld(SDL_Keycode.SDLK_Q))
        {
            Window.ExitProgram();
        }

        BoxSelect.Update(Renderer, Window.GetInputManager().Mouse);

        BasicButton.Update(deltaTime, Window.GetInputManager().Mouse);

        Window.GetInputManager().Clear();
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:
        
    }

    public void Render(float deltaTime)
    {
        // TODO: Render game elements to the screen below:

        PinkFoxLogo.Render(Renderer);
        DvdLogo.Render(Renderer);
        BoxSelect.Render(Renderer);

        Shape.Render(Renderer, new FRect(0, 0, Shape.Width, Shape.Height), new FRect(100, 100, Shape.Width, Shape.Height));
        Shape.Render(Renderer, new FRect(0, 0, Shape.Width, Shape.Height), new FRect(150, 150, Shape.Width, Shape.Height));
        TestLabel.Render(Renderer);

        BasicButton.Render(Renderer);
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