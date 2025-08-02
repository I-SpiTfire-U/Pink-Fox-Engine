using PinkFox.Audio;
using PinkFox.Core.Interfaces;
using PinkFox.Core.Physics;
using PinkFox.Graphics.Rendering;
using PinkFox.Input;
using PinkFox.Input.InputDevices;
using SDL3;

namespace PinkFox.SampleGame;

public class Player : Sprite2D
{
    public Animation? Animation = null;
    private float _MoveVelocity = 0f;
    private float _MoveAcceleration;
    private float _MoveVelocityMax;
    private float _JumpVelocity;
    private float _Gravity;
    private float _VerticalVelocity = 0f;
    private bool _IsGrounded = false;

    public Player(Texture2D texture, float x, float y, float gravity, float jumpVelocity, float moveAcceleration, float moveVelocityMax, float scale = 1, float rotation = 0, SDL.FRect? sourceRect = null, SDL.FlipMode flipMode = SDL.FlipMode.None, bool isVisible = true)
    : base(texture, x, y, scale, rotation, sourceRect, flipMode, isVisible)
    {
        _Gravity = gravity;
        _JumpVelocity = jumpVelocity;
        _MoveAcceleration = moveAcceleration;
        _MoveVelocityMax = moveVelocityMax;
    }

    public Player(Texture2D texture, float x, float y, float gravity, float jumpVelocity, float moveAcceleration, float moveVelocityMax, float scaleX = 1, float scaleY = 1, float rotation = 0, SDL.FRect? sourceRect = null, SDL.FlipMode flipMode = SDL.FlipMode.None, bool isVisible = true)
    : base(texture, x, y, scaleX, scaleY, rotation, sourceRect, flipMode, isVisible)
    {
        _Gravity = gravity;
        _JumpVelocity = jumpVelocity;
        _MoveAcceleration = moveAcceleration;
        _MoveVelocityMax = moveVelocityMax;
    }

    public void Update(float deltaTime, List<Sprite2D> sprites, IInputManager inputManager, IAudioManager audioManager)
    {
        IKeyboard keyboard = inputManager.Keyboard;

        float moveAcceleration = _MoveAcceleration;
        float moveDeceleration = 1000f;

        float targetVelocity = 0f;

        _VerticalVelocity += _Gravity * deltaTime;
        MoveVertically(_VerticalVelocity * deltaTime, sprites);

        if (keyboard.IsKeyHeld(SDL.Keycode.Space) || (inputManager.Gamepads.AtIndex(0)?.IsButtonHeld(SDL.GamepadButton.South) ?? false))
        {
            Jump(_JumpVelocity, audioManager);
        }

        bool isMoving = false;
        if (keyboard.IsKeyHeld(SDL.Keycode.A) || inputManager.Gamepads.AtIndex(0)?.GetAxisFiltered(SDL.GamepadAxis.LeftX) < 0)
        {
            targetVelocity = -_MoveVelocityMax;
            isMoving = true;
        }
        if (keyboard.IsKeyHeld(SDL.Keycode.D) || inputManager.Gamepads.AtIndex(0)?.GetAxisFiltered(SDL.GamepadAxis.LeftX) > 0)
        {
            targetVelocity = _MoveVelocityMax;
            isMoving = true;
        }

        if (isMoving)
        {
            float direction = MathF.Sign(targetVelocity);
            float currentDir = MathF.Sign(_MoveVelocity);

            bool reversing = _MoveVelocity != 0 && direction != currentDir;
            float acceleration = reversing ? moveAcceleration * 2.5f : moveAcceleration;

            if (_MoveVelocity > targetVelocity)
            {
                _MoveVelocity -= acceleration * deltaTime;
                if (_MoveVelocity < targetVelocity)
                {
                    _MoveVelocity = targetVelocity;
                }
            }
            else if (_MoveVelocity < targetVelocity)
            {
                _MoveVelocity += acceleration * deltaTime;
                if (_MoveVelocity > targetVelocity)
                {
                    _MoveVelocity = targetVelocity;
                }
            }
        }
        else
        {
            if (_MoveVelocity > 0)
            {
                _MoveVelocity -= moveDeceleration * deltaTime;
                if (_MoveVelocity < 0)
                {
                    _MoveVelocity = 0;
                }
            }
            else if (_MoveVelocity < 0)
            {
                _MoveVelocity += moveDeceleration * deltaTime;
                if (_MoveVelocity > 0)
                {
                    _MoveVelocity = 0;
                }
            }
        }

        if (MathF.Abs(_MoveVelocity) > 0.01f)
        {
            MoveHorizontally(_MoveVelocity * deltaTime, sprites);
        }

        if (_IsGrounded)
        {
            Animation?.SetCurrentFrame(_MoveVelocity >= _MoveVelocityMax || _MoveVelocity <= -_MoveVelocityMax ? 2 : 0);
        }
        else
        {
            Animation?.SetCurrentFrame(1);
        }

        if (Animation is not null)
        {
            SetSourceRect(Animation.GetCurrentFrame());
        }
    }

    public new void Draw(nint renderer, ICamera2D camera2D)
    {
        base.Draw(renderer, camera2D);
    }

    public void Jump(float jumpVelocity, IAudioManager audioManager)
    {
        if (!_IsGrounded)
        {
            return;
        }

        audioManager.PlaySound("Jump");
        _VerticalVelocity = -jumpVelocity;
        _IsGrounded = false;
    }

    private void MoveHorizontally(float amount, List<Sprite2D> sprites)
    {
        float x = Position.X + amount;

        foreach (Sprite2D sprite in sprites)
        {
            if (Collider.IsCollidingWith(sprite.Collider))
            {
                CollisionDirection dir = Collision.GetCollisionDirection(Collider, sprite.Collider);
                if (dir == CollisionDirection.Left && amount < 0)
                {
                    x = sprite.Collider.Right;
                    _MoveVelocity = 0f;
                    break;
                }
                if (dir == CollisionDirection.Right && amount > 0)
                {
                    x = sprite.Collider.Left - Width;
                    _MoveVelocity = 0f;
                    break;
                }
            }
        }

        Position = new(x, Position.Y);
    }

    private void MoveVertically(float amount, List<Sprite2D> sprites)
    {
        float y = Position.Y + amount;
        bool isGrounded = false;

        foreach (Sprite2D sprite in sprites)
        {
            if (Collider.IsCollidingWith(sprite.Collider))
            {
                CollisionDirection dir = Collision.GetCollisionDirection(Collider, sprite.Collider);
                if (dir == CollisionDirection.Bottom && amount > 0)
                {
                    y = sprite.Collider.Top - Height + 1;
                    _VerticalVelocity = 0f;
                    isGrounded = true;
                    break;
                }
                if (dir == CollisionDirection.Top && amount < 0)
                {
                    y = sprite.Collider.Bottom;
                    _VerticalVelocity = 0f;
                    break;
                }
            }
        }

        _IsGrounded = isGrounded;
        Position = new(Position.X, y);
    }
}