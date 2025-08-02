using SDL3;
using PinkFox.Core.Scenes;
using PinkFox.Core.Interfaces;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Cameras;
using System.Numerics;

namespace PinkFox.SampleGame.Scenes;

public class SampleScene : IScene, IDisposable
{
    protected IInputManager InputManager;
    protected IAudioManager AudioManager;
    protected List<Sprite2D> GameObjects;
    protected readonly Player PlayerObject;
    protected readonly Camera2D Camera;
    protected Vector2 LastPlayerCenter;
    private bool _Disposed;
    public event Action? OnRequestExit;

    public SampleScene(nint renderer, int windowWidth, int windowHeight, IInputManager inputManager, IAudioManager audioManager)
    {
        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(windowWidth, windowHeight);

        InputManager = inputManager;
        AudioManager = audioManager;

        Texture2D playerTexture = new(@"Assets\Images\Standing.png", renderer);
        Texture2D playerJumpTexture = new(@"Assets\Images\Jumping.png", renderer);
        PlayerObject = new(playerTexture, 0f, 0f, 500f, 400f, 400f, 400f, scale: 0.3f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        PlayerObject.SetJumpTexture(playerJumpTexture);

        GameObjects = [];

        Texture2D groundTexture = new(@"C:\Users\Lex\Media\Pictures\Saved Pictures\1748496217430.jpg", renderer);
        Sprite2D GroundSprite = new(groundTexture, 0f, 300f, scaleX: 0.5f, scaleY: 0.1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        Sprite2D RoofSprite = new(groundTexture, 200f, 150f, scaleX: 0.5f, scaleY: 0.1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);
        Sprite2D WallSprite = new(groundTexture, -32f, 80f, scaleX: 0.03f, scaleY: 1f, rotation: 0f, sourceRect: null, flipMode: SDL.FlipMode.None);

        GameObjects.AddRange(GroundSprite, RoofSprite, WallSprite);

        AudioManager.LoadSound("Jump", @"Assets\Audio\Jump.wav");
        AudioManager.LoadTrack("Louvre Museum Invasion", @"Assets\Audio\Louvre Museum Invasion.mp3");

        AudioManager.SetSoundVolume("Jump", 0.3f);
        AudioManager.SetMusicVolume(0.1f);

        AudioManager.PlayMusic("Louvre Museum Invasion");
    }

    public void Update(float deltaTime)
    {
        // Update game objects and code below:

        PlayerObject.Update(deltaTime, GameObjects, InputManager, AudioManager);

        if (InputManager.Keyboard.IsKeyHeld(SDL.Keycode.Up) || (InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.RightShoulder) ?? false))
        {
            Camera.ChangeZoom(1f * deltaTime);
        }
        if (InputManager.Keyboard.IsKeyHeld(SDL.Keycode.Down)  || (InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.LeftShoulder) ?? false))
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
        // Update physics code and fixed values here:

    }

    public void Draw(nint renderer)
    {
        // Draw graphics to the screen below:

        PlayerObject.Draw(renderer, Camera);

        foreach (Sprite2D sprite in GameObjects)
        {
            sprite.Draw(renderer, Camera);
        }
    }

    public void OnWindowResize(int width, int height)
    {
        // Adjust camera, UI elements, screen-space effects, etc.

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