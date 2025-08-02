using SDL3;
using PinkFox.Core.Scenes;
using PinkFox.Core.Components;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Cameras;
using System.Numerics;

namespace PinkFox.SampleGame.Scenes;

public class SampleScene : IScene, IDisposable
{
    protected float CurrentFPS = 0f;
    protected float FPSTimer = 0f;
    protected int FrameCount = 0;

    public event Action? OnRequestExit;

    protected readonly IInputManager InputManager;
    protected readonly IAudioManager AudioManager;

    protected readonly Camera2D Camera;

    protected readonly Font BasicFont;
    protected readonly Texture2D PlayerTexture;
    protected readonly Texture2D GroundTexture;

    protected readonly Player PlayerObject;
    protected readonly List<Sprite2D> GameObjects;

    private bool _Disposed;

    public SampleScene(nint renderer, int windowWidth, int windowHeight, IInputManager inputManager, IAudioManager audioManager)
    {
        InputManager = inputManager;
        AudioManager = audioManager;

        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(windowWidth, windowHeight);

        BasicFont     = new(@"Assets\Fonts\arial.ttf", 16f);
        PlayerTexture = new(@"Assets\Images\SpriteAtlas.png", renderer);
        GroundTexture = new(@"C:\Users\Lex\Media\Pictures\Saved Pictures\1748496217430.jpg", renderer);

        PlayerObject = new(PlayerTexture, new Vector2(0f, 0f), 500f, 400f, 400f, 400f, scale: 0.3f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None)
        {
            Animation = new Animation(266, 266, 2)
        };

        Sprite2D GroundSprite   = new(GroundTexture, new Vector2(  0f, 300f), scaleX:  0.5f, scaleY: 0.1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        Sprite2D RoofSprite     = new(GroundTexture, new Vector2(200f, 150f), scaleX:  0.5f, scaleY: 0.1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        Sprite2D WallSprite     = new(GroundTexture, new Vector2(-32f,  80f), scaleX: 0.03f, scaleY:   1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);

        GameObjects = [ GroundSprite, RoofSprite, WallSprite ];
    }

    public void LoadContent()
    {
        // TODO: Load game content below:

        // Audio
        AudioManager.LoadSound("Jump", @"Assets\Audio\Jump.wav");
        AudioManager.LoadTrack("Louvre Museum Invasion", @"Assets\Audio\Louvre Museum Invasion.mp3");

        AudioManager.SetSoundVolume("Jump", 0.3f);
        AudioManager.SetMusicVolume(0.1f);
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game objects and code below:

        UpdateFPS(deltaTime);

        if (!AudioManager.IsMusicPlaying)
        {
            AudioManager.PlayMusic("Louvre Museum Invasion");
        }

        PlayerObject.Update(deltaTime, GameObjects, InputManager, AudioManager);

        if (InputManager.Keyboard.IsKeyHeld(SDL.Keycode.Up) || (InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.RightShoulder) ?? false))
        {
            Camera.UpdateZoom(1f * deltaTime);
        }
        if (InputManager.Keyboard.IsKeyHeld(SDL.Keycode.Down) || (InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.LeftShoulder) ?? false))
        {
            Camera.UpdateZoom(-1f * deltaTime);
        }

        Camera.SetPosition(PlayerObject.Center - new Vector2(Camera.ViewWidth, Camera.ViewHeight) * 0.5f / Camera.Zoom);

        if (InputManager.Mouse.Collider.IsCollidingWith(PlayerObject.Collider) && InputManager.Mouse.IsButtonDown(SDL.MouseButtonFlags.Left))
        {
            OnRequestExit?.Invoke();
        }

        if (InputManager.Keyboard.IsKeyDown(SDL.Keycode.Escape) || (InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.Start) ?? false))
        {
            OnRequestExit?.Invoke();
        }
    }

    public void FixedUpdate()
    {
        // TODO: Update physics code and fixed values here:

    }

    public void Draw(nint renderer)
    {
        // TODO: Draw graphics to the screen below:

        PlayerObject.Draw(renderer, Camera);

        foreach (Sprite2D sprite in GameObjects)
        {
            sprite.Draw(renderer, Camera);
        }

        BasicFont.DrawText(renderer, $"FPS: {CurrentFPS:F2}");
    }

    private void UpdateFPS(float deltaTime)
    {
        FrameCount++;
        FPSTimer += deltaTime;

        if (FPSTimer < 1f)
        {
            return;
        }

        CurrentFPS = FrameCount / FPSTimer;
        FrameCount = 0;
        FPSTimer = 0f;
    }

    public void OnWindowResize(int width, int height)
    {
        // TODO: Adjust camera, UI elements, screen-space effects, etc.

        Camera.SetViewSize(width, height);
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
        
        if (disposing)
        {
            PlayerObject.Dispose();
        }

        _Disposed = true;
    }
}