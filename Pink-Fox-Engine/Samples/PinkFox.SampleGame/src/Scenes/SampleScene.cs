using SDL3;
using PinkFox.Core.Scenes;
using PinkFox.Core.Components;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Cameras;
using System.Numerics;

namespace PinkFox.SampleGame.Scenes;

public class SampleScene : IScene, IDisposable
{
    protected Font BasicFont = new(@"Assets\Fonts\arial.ttf", 16f);
    protected IInputManager InputManager;
    protected IAudioManager AudioManager;
    protected List<Sprite2D> GameObjects;
    protected readonly Player PlayerObject;
    protected readonly Camera2D Camera;
    protected Vector2 LastPlayerCenter;
    private bool _Disposed;
    public event Action? OnRequestExit;

    public float CurrentFPS = 0f;
    private float _FPSTimer = 0f;
    private int _FrameCount = 0;

    public SampleScene(nint renderer, int windowWidth, int windowHeight, IInputManager inputManager, IAudioManager audioManager)
    {
        InputManager = inputManager;
        AudioManager = audioManager;

        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(windowWidth, windowHeight);

        Texture2D playerTexture = new(@"Assets\Images\SpriteAtlas.png", renderer);
        PlayerObject = new(playerTexture, 0f, 0f, 500f, 400f, 400f, 400f, scale: 0.3f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        Animation playerAnimation = new(266, 266, 2);
        PlayerObject.Animation = playerAnimation;
        GameObjects = [];

        Texture2D groundTexture = new(@"C:\Users\Lex\Media\Pictures\Saved Pictures\1748496217430.jpg", renderer);
        Sprite2D GroundSprite = new(groundTexture, 0f, 300f, scaleX: 0.5f, scaleY: 0.1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        Sprite2D RoofSprite = new(groundTexture, 200f, 150f, scaleX: 0.5f, scaleY: 0.1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        Sprite2D WallSprite = new(groundTexture, -32f, 80f, scaleX: 0.03f, scaleY: 1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);

        GameObjects.AddRange(GroundSprite, RoofSprite, WallSprite);
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
            Camera.ChangeZoom(1f * deltaTime);
        }
        if (InputManager.Keyboard.IsKeyHeld(SDL.Keycode.Down) || (InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.LeftShoulder) ?? false))
        {
            Camera.ChangeZoom(-1f * deltaTime);
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
        _FrameCount++;
        _FPSTimer += deltaTime;

        if (_FPSTimer < 1f)
        {
            return;
        }

        CurrentFPS = _FrameCount / _FPSTimer;
        _FrameCount = 0;
        _FPSTimer = 0f;
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

    private void Dispose(bool disposing)
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