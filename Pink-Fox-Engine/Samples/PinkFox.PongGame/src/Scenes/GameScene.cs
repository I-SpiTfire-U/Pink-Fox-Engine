using SDL;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Cameras;
using PinkFox.Core;
using PongGame.GameObjects;
using PinkFox.UI.Text;
using System.Numerics;
using PinkFox.Core.Collisions;
using PinkFox.Input;

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

    protected readonly Font ArialFont;
    protected readonly Label PlayerOneScoreLabel;
    protected readonly Label PlayerTwoScoreLabel;

    protected readonly Texture2D RectangleTexture;
    protected readonly PongBall Ball;
    protected readonly PlayerPaddle PlayerOne;
    protected readonly PlayerPaddle PlayerTwo;

    protected readonly Vector2 BallSize;
    protected readonly Vector2 PaddleSize;
    protected readonly Vector2 BallStartPosition;
    protected readonly Vector2 PlayerOneStartPosition;
    protected readonly Vector2 PlayerTwoStartPosition;

    private bool _Disposed;

    public unsafe GameScene(Engine engine)
    {
        Engine = engine;
        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(Engine.WindowWidth, Engine.WindowHeight);

        ArialFont = new(@"Assets\Fonts\arial.ttf", 32f);
        PlayerOneScoreLabel = new(Engine.Renderer, ArialFont, "0", new Vector2(Engine.WindowCenter.X - 100, 50));
        PlayerTwoScoreLabel = new(Engine.Renderer, ArialFont, "0", new Vector2(Engine.WindowCenter.X + 100, 50));

        BallSize = new(20, 20);
        PaddleSize = new(15, 80);
        BallStartPosition = new(Engine.WindowCenter.X - 10, Engine.WindowCenter.Y - 10);
        PlayerOneStartPosition = new(0, Engine.WindowCenter.Y - 40);
        PlayerTwoStartPosition = new(Engine.WindowWidth - 15, Engine.WindowCenter.Y - 40);

        RectangleTexture = TextureFactory.CreateRectangle(Engine.Renderer, 1, 1, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });

        Ball = new("Ball", RectangleTexture, BallStartPosition, BallSize);
        PlayerOne = new("Player One", RectangleTexture, PlayerOneStartPosition, PaddleSize, SDL_Keycode.SDLK_W, SDL_Keycode.SDLK_S, 0);
        PlayerTwo = new("Player Two", RectangleTexture, PlayerTwoStartPosition, PaddleSize, SDL_Keycode.SDLK_UP, SDL_Keycode.SDLK_DOWN, 1);
    }

    public void LoadContent()
    {
        Engine.AudioManager.LoadSound("Collision", @"Assets\Audio\Collision.wav");
        Engine.AudioManager.LoadSound("Goal", @"Assets\Audio\Goal.wav");

        Engine.AudioManager.SetSoundVolume("Collision", 1f);
        Engine.AudioManager.SetSoundVolume("Goal", 1f);
    }

    public unsafe void Update(float deltaTime)
    {
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

        PlayerOneScoreLabel.Draw();
        PlayerTwoScoreLabel.Draw();
    }

    public void OnWindowResize(int windowWidth, int windowHeight)
    {
        Camera.SetViewSize(windowWidth, windowHeight);
    }

    private bool CheckForGoal()
    {
        if (Ball.Center.X > GoalBufferWidth && Ball.Center.X < Engine.VirtualRenderer.VirtualWidth - GoalBufferWidth)
        {
            return false;
        }

        if (Ball.Position.X < 0f)
        {
            PlayerTwoScore++;
            PlayerTwoScoreLabel.SetText(PlayerTwoScore.ToString());
        }

        if (Ball.Position.X > 0f)
        {
            PlayerOneScore++;
            PlayerOneScoreLabel.SetText(PlayerOneScore.ToString());
        }

        Engine.AudioManager.PlaySound("Goal");
        ResetGameObjects();
        return true;
    }

    private void ResetGameObjects()
    {
        Ball.Reset(BallStartPosition);
        PlayerOne.Reset(PlayerOneStartPosition);
        PlayerTwo.Reset(PlayerTwoStartPosition);
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
            PlayerOneScoreLabel.Dispose();
            PlayerTwoScoreLabel.Dispose();
            ArialFont.Dispose();

            Ball.Dispose();
            PlayerOne.Dispose();
            PlayerTwo.Dispose();
            RectangleTexture.Dispose();
        }

        _Disposed = true;
    }
}