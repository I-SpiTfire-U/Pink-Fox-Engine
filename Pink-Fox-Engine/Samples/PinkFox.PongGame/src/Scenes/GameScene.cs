using SDL;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Cameras;
using PinkFox.Core;
using PongGame.GameObjects;
using System.Numerics;
using PinkFox.Graphics.Fonts;

namespace PongGame.Scenes;

public class GameScene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected bool PlayerTwoIsAI = true;
    protected int PlayerOneScore = 0;
    protected int PlayerTwoScore = 0;

    protected readonly float GoalBufferWidth = 10f;

    protected readonly Engine Engine;
    protected readonly Camera2D Camera;

    protected BitmapFontData GameFontData;
    protected BitmapFont GameFont;
    protected Texture2D GameFontTexture;
    protected readonly Label PlayerOneScoreLabel;
    protected readonly Label PlayerTwoScoreLabel;
    protected Label FPSLabel;

    protected readonly Texture2D RectangleTexture;
    protected readonly PongBall Ball;
    protected readonly PlayerPaddle PlayerOne;
    protected readonly PlayerPaddle PlayerTwo;

    private bool _Disposed;

    public unsafe GameScene(Engine engine)
    {
        Engine = engine;
        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(Engine.WindowWidth, Engine.WindowHeight);

        GameFontTexture = new("Arial_Regular_32", Engine.Renderer);
        GameFontData = BitmapFont.LoadFontDataFromJson("Json_Arial_Regular_32");
        GameFont = new(GameFontTexture, GameFontData);

        PlayerOneScoreLabel = new(GameFont, "0", 32, new Vector2(Engine.WindowCenter.X - 100, 50));
        PlayerTwoScoreLabel = new(GameFont, "0", 32, new Vector2(Engine.WindowCenter.X + 100, 50));
        FPSLabel = new(GameFont, null, 16, new(0, 0));

        RectangleTexture = TextureFactory.CreateRectangle(Engine.Renderer, 1, 1, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });

        Ball = new("Ball", RectangleTexture, new(Engine.WindowCenter.X - 10, Engine.WindowCenter.Y - 10), new(20, 20));
        PlayerOne = new("Player One", RectangleTexture, new(0, Engine.WindowCenter.Y - 40), new(15, 80), SDL_Keycode.SDLK_W, SDL_Keycode.SDLK_S, 0);
        PlayerTwo = new("Player Two", RectangleTexture, new(Engine.WindowWidth - 15, Engine.WindowCenter.Y - 40), new(15, 80), SDL_Keycode.SDLK_UP, SDL_Keycode.SDLK_DOWN, 1);
    }

    public void LoadContent()
    {
        Engine.AudioManager.LoadSound("Collision", "Audio_Collision");
        Engine.AudioManager.LoadSound("Goal", "Audio_Goal");

        Engine.AudioManager.SetSoundVolume("Collision", 1f);
        Engine.AudioManager.SetSoundVolume("Goal", 1f);
    }

    public unsafe void Update(float deltaTime)
    {
        UpdateFps(deltaTime);
        PlayerOneScoreLabel.Update(deltaTime);
        PlayerTwoScoreLabel.Update(deltaTime);

        if (Engine.InputManager.Keyboard.IsKeyDown(SDL_Keycode.SDLK_ESCAPE) || Engine.InputManager.Gamepads.IsButtonHeld(0, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START))
        {
            Engine.Stop();
        }

        if (Engine.InputManager.Keyboard.IsKeyDown(SDL_Keycode.SDLK_SPACE))
        {
            ResetGameObjects();
            PlayerTwoIsAI = !PlayerTwoIsAI;
        }

        if (Engine.InputManager.Gamepads.Count > 1 && !PlayerTwoIsAI)
        {
            ResetGameObjects();
            PlayerTwoIsAI = true;
        }

        Ball.Update(deltaTime, Engine.VirtualRenderer.VirtualHeight, Engine.AudioManager);
        PlayerOne.Update(deltaTime, Engine.VirtualRenderer.VirtualHeight, Engine.InputManager, Ball, false);
        PlayerTwo.Update(deltaTime, Engine.VirtualRenderer.VirtualHeight, Engine.InputManager, Ball, PlayerTwoIsAI);

        Ball.OnCollisionWithPaddle(PlayerOne, Engine.AudioManager);
        Ball.OnCollisionWithPaddle(PlayerTwo, Engine.AudioManager);

        if (CheckForGoal())
        {
            return;
        }
    }

    public void FixedUpdate(float fixedUpdateInterval) { }

    public unsafe void Draw(SDL_Renderer* renderer)
    {
        PlayerOne.Draw(renderer, Camera);
        PlayerTwo.Draw(renderer, Camera);
        Ball.Draw(renderer, Camera);

        PlayerOneScoreLabel.Draw(renderer, Camera);
        PlayerTwoScoreLabel.Draw(renderer, Camera);
        FPSLabel.Draw(renderer, Camera);
    }

    public void OnWindowResize(int windowWidth, int windowHeight)
    {
        Camera.SetViewSize(windowWidth, windowHeight);
    }

    private bool CheckForGoal()
    {
        if (Ball.Center.X < 0f)
        {
            PlayerTwoScore++;
            PlayerTwoScoreLabel.SetText(PlayerTwoScore.ToString());
            PlayerTwoScoreLabel.Shake(1f, 1f);
        }
        else if (Ball.Center.X > Engine.VirtualRenderer.VirtualWidth)
        {
            PlayerOneScore++;
            PlayerOneScoreLabel.SetText(PlayerOneScore.ToString());
            PlayerOneScoreLabel.Shake(1f, 1f);
        }
        else
        {
            return false;
        }

        Engine.AudioManager.PlaySound("Goal");
        ResetGameObjects();
        return true;
    }

    private void ResetGameObjects()
    {
        Ball.Reset();
        PlayerOne.Reset();
        PlayerTwo.Reset();
    }

    private int _FrameCount = 0;
    private float _ElapsedTime = 0f;
    private float _Fps = 0;

    public void UpdateFps(float deltaTime)
    {
        _FrameCount++;
        _ElapsedTime += deltaTime;

        if (_ElapsedTime >= 1f)
        {
            _Fps = _FrameCount;
            _FrameCount = 0;
            _ElapsedTime = 0f;
        }

        FPSLabel.SetText($"FPS: {_Fps:F2}");
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
            GameFontTexture.Dispose();
            Ball.Dispose();
            PlayerOne.Dispose();
            PlayerTwo.Dispose();
            RectangleTexture.Dispose();
        }

        _Disposed = true;
    }
}