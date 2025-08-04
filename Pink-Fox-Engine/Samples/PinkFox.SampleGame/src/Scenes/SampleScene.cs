using SDL3;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Cameras;
using System.Numerics;
using PinkFox.Core;
using PinkFox.UI.Text;

namespace PinkFox.SampleGame.Scenes;

public class SampleScene : IScene, IDisposable
{
    protected readonly Engine Engine;
    protected float CurrentFPS = 0f;
    protected float FPSTimer = 0f;
    protected int FrameCount = 0;

    public event Action? OnRequestExit;

    protected readonly Camera2D Camera;

    protected readonly Font ArialFont;
    protected readonly Texture2D PlayerTexture;
    protected readonly Texture2D GroundTexture;

    protected readonly PlayerObject PlayerObject;
    protected readonly List<ISprite2D> SpritePool;

    private bool _Disposed;

    public SampleScene(Engine engine)
    {
        Engine = engine;

        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(Engine.WindowWidth, Engine.WindowHeight);

        ArialFont = new(@"Assets\Fonts\arial.ttf", 16f);

        PlayerTexture = new(@"Assets\Images\Player.png", Engine.Renderer);
        PlayerObject = new(
            name: "Player",
            horizontalVelocity: new Velocity(400f, 400f, 0f),
            verticalVelocity: new Velocity(500f, 500f, 0f),
            jumpForce: 400f,
            texture: PlayerTexture,
            animations: new()
            {
                { "DefaultAnimation", new Animation(266, 266, PlayerTexture)}
            },
            position: new Vector2(0f, 0f),
            origin: null,
            scale: new Vector2(96f),
            rotation: 0f,
            flipMode: SDL.FlipMode.None,
            isVisible: true
        );

        GroundTexture = new(@"Assets\Images\Block.png", Engine.Renderer);
        Sprite2D GroundSprite = new(
            name: "Ground1",
            texture: GroundTexture,
            position: new Vector2(0f, 64),
            origin: null,
            sourceRect: null,
            scale: new Vector2(256f, 32f),
            rotation: 0f,
            flipMode: SDL.FlipMode.None,
            isVisible: true
        );
        Sprite2D RoofSprite = new(
            name: "Ground2",
            texture: GroundTexture,
            position: new Vector2(GroundSprite.Position.X + GroundSprite.Scale.X, -70f),
            origin: null,
            sourceRect: null,
            scale: new Vector2(256f, 32f),
            rotation: 0f,
            flipMode: SDL.FlipMode.None,
            isVisible: true
        );
        Sprite2D WallSprite = new(
            name: "Wall1",
            texture: GroundTexture,
            position: new Vector2(-32f, -160f),
            origin: null,
            sourceRect: null,
            scale: new Vector2(32f, 256f),
            rotation: 0f,
            flipMode: SDL.FlipMode.None,
            isVisible: true
        );

        SpritePool = [ GroundSprite, RoofSprite, WallSprite ];
    }

    public void LoadContent()
    {
        // TODO: Load game content below:

        // Audio
        Engine.AudioManager.LoadSound("Jump", @"Assets\Audio\Jump.wav");
        Engine.AudioManager.LoadTrack("Louvre Museum Invasion", @"Assets\Audio\Louvre Museum Invasion.mp3");

        Engine.AudioManager.SetSoundVolume("Jump", 0.3f);
        Engine.AudioManager.SetMusicVolume(0.1f);
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game objects and code below:

        UpdateFPS(deltaTime);

        if (!Engine.AudioManager.IsMusicPlaying)
        {
            Engine.AudioManager.PlayMusic("Louvre Museum Invasion");
        }

        PlayerObject.Update(deltaTime, Engine.InputManager);

        if (Engine.InputManager.Keyboard.IsKeyHeld(SDL.Keycode.Up) || (Engine.InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.RightShoulder) ?? false))
        {
            Camera.UpdateZoom(1f * deltaTime);
        }
        if (Engine.InputManager.Keyboard.IsKeyHeld(SDL.Keycode.Down) || (Engine.InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.LeftShoulder) ?? false))
        {
            Camera.UpdateZoom(-1f * deltaTime);
        }

        Camera.SetPosition(PlayerObject.Origin - Camera.ViewOffset);

        if (Engine.InputManager.Mouse.Collider.IsCollidingWith(PlayerObject.Collider) && Engine.InputManager.Mouse.IsButtonDown(SDL.MouseButtonFlags.Left))
        {
            OnRequestExit?.Invoke();
        }

        if (Engine.InputManager.Keyboard.IsKeyDown(SDL.Keycode.Escape) || (Engine.InputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.Start) ?? false))
        {
            OnRequestExit?.Invoke();
        }
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics code and fixed values here:

        PlayerObject.FixedUpdate(fixedUpdateInterval, SpritePool, Engine.AudioManager);
    }

    public void Draw(nint renderer, float alpha)
    {
        // TODO: Draw graphics to the screen below:

        ArialFont.DrawText(renderer, $"FPS: {CurrentFPS:F2}");

        PlayerObject.Draw(renderer, Camera, alpha);

        foreach (ISprite2D sprite in SpritePool)
        {
            if (Camera.SpriteIsInView(sprite))
            {
                sprite.Draw(renderer, Camera);
                //Console.WriteLine($"Sprite Rendered {sprite.Name}");
            }
        }
    }

    private void UpdateFPS(float deltaTime)
    {
        FrameCount++;
        FPSTimer += deltaTime;

        if (FPSTimer < 0.1f)
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