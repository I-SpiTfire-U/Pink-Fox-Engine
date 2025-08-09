using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Sprites;
using SDL;

namespace PongGame.GameObjects;

public class PlayerPaddle : Sprite2D, ISprite2D
{
    private readonly SDL_Keycode _UpKey;
    private readonly SDL_Keycode _DownKey;
    private readonly int _GamepadIndex;

    private const float MoveSpeed = 250f;

    public PlayerPaddle(string name, Texture2D texture, Vector2 position, Vector2 scale, SDL_Keycode upKey, SDL_Keycode downKey, int gamepadIndex, bool isVisible = true)
    : base(name, texture, position, null, null, scale, 0d, SDL_FlipMode.SDL_FLIP_NONE, 0, isVisible)
    {
        _UpKey = upKey;
        _DownKey = downKey;
        _GamepadIndex = gamepadIndex;
    }

    public void Update(float deltaTime, float windowHeight, IInputManager inputManager, PongBall ball, bool isAI)
    {
        float newYPosition = Position.Y;

        if (inputManager.Keyboard.IsKeyHeld(_UpKey) || inputManager.Gamepads.IsButtonHeld(_GamepadIndex, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_UP) || (isAI && ball.Center.Y < Center.Y))
        {
            newYPosition -= MoveSpeed * deltaTime;
        }

        if (inputManager.Keyboard.IsKeyHeld(_DownKey) || inputManager.Gamepads.IsButtonHeld(_GamepadIndex, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_DPAD_DOWN) || (isAI && ball.Center.Y > Center.Y))
        {
            newYPosition += MoveSpeed * deltaTime;
        }

        Position = new(Position.X, Math.Clamp(newYPosition, 0f, windowHeight - Scale.Y));
    }

    public void Reset(Vector2 position)
    {
        Position = position;
    }
}