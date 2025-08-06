using PinkFox.Core.Components;
using PinkFox.Core.Collisions;
using PinkFox.Graphics.Rendering;
using System.Numerics;
using PinkFox.Core.Scenes;
using SDL;

namespace PinkFox.SampleGame;

public class PlayerObject : AnimatedSprite2D, ISprite2D
{
    private Vector2 _PreviousPosition;
    private bool _JumpRequested = false;
    private float _MoveDirection = 0f;
    private readonly Velocity _HorizontalVelocity;
    private readonly Velocity _VerticalVelocity;
    private readonly float _JumpForce;

    private bool _IsGrounded = false;

    public PlayerObject(string name, Velocity horizontalVelocity, Velocity verticalVelocity, float jumpForce, Texture2D texture, Dictionary<string, Animation> animations, Vector2 position, Vector2? origin = null, Vector2? scale = null, double rotation = 0f, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE, bool isVisible = true)
    : base(name, texture, animations, position, origin, scale, rotation, flipMode, isVisible)
    {
        _HorizontalVelocity = horizontalVelocity;
        _VerticalVelocity = verticalVelocity;
        _JumpForce = jumpForce;
    }

    public void Update(float deltaTime, IInputManager inputManager)
    {
        _PreviousPosition = Position;
        _MoveDirection = 0f;

        if (inputManager.Keyboard.IsKeyDown(SDL_Keycode.SDLK_SPACE) || (inputManager.Gamepads.AtIndex(0)?.IsButtonDown(SDL_GamepadButton.SDL_GAMEPAD_BUTTON_SOUTH) ?? false))
        {
            _JumpRequested = true;
        }
        if (inputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_A) || inputManager.Gamepads.AtIndex(0)?.GetAxisFiltered(SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX) < 0)
        {
            _MoveDirection = -1f;
        }
        if (inputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_D) || inputManager.Gamepads.AtIndex(0)?.GetAxisFiltered(SDL_GamepadAxis.SDL_GAMEPAD_AXIS_LEFTX) > 0)
        {
            _MoveDirection = 1f;
        }
    }

    public void FixedUpdate(float fixedUpdateInterval, List<ISprite2D> spritePool, IAudioManager audioManager)
    {
        UpdateVerticalValues(fixedUpdateInterval, spritePool, audioManager);
        UpdateHorizontalValues(fixedUpdateInterval, spritePool);
        CorrectSmallOverlaps(spritePool);

        if (!_IsGrounded)
        {
            SetCurrentFrame(1);
            return;
        }

        if (_HorizontalVelocity.CurrentVelocity >= _HorizontalVelocity.MaximumVelocity || _HorizontalVelocity.CurrentVelocity <= -_HorizontalVelocity.MaximumVelocity)
        {
            SetCurrentFrame(2);
            return;
        }
        
        SetCurrentFrame(0);
    }

    public unsafe void Draw(SDL_Renderer* renderer, ICamera2D? camera2D = null, float alpha = 1f)
    {
        Vector2 originalPosition = Position;
        Position = Vector2.Lerp(_PreviousPosition, originalPosition, alpha);
        base.Draw(renderer, camera2D);
        Position = originalPosition;
    }

    private void UpdateVerticalValues(float fixedUpdateInterval, List<ISprite2D> spritePool, IAudioManager audioManager)
    {
        _VerticalVelocity.CurrentVelocity += _VerticalVelocity.AccelerationRate * fixedUpdateInterval;

        if (_JumpRequested)
        {
            Jump(_JumpForce, audioManager);
            _JumpRequested = false;
        }

        MoveVertically(_VerticalVelocity.CurrentVelocity * fixedUpdateInterval, spritePool);
    }

    private void UpdateHorizontalValues(float fixedUpdateInterval, List<ISprite2D> spritePool)
    {
        float moveAcceleration = _HorizontalVelocity.AccelerationRate;
        float moveDeceleration = _HorizontalVelocity.AccelerationRate * 3;
        float targetVelocity = 0f;

        bool isMoving = false;
        if (_MoveDirection != 0f)
        {
            targetVelocity = _MoveDirection * _HorizontalVelocity.MaximumVelocity;
            isMoving = true;
        }

        if (isMoving)
        {
            float direction = MathF.Sign(targetVelocity);
            float currentDir = MathF.Sign(_HorizontalVelocity.CurrentVelocity);

            bool reversing = _HorizontalVelocity.CurrentVelocity != 0 && direction != currentDir;
            float acceleration = reversing ? moveAcceleration * 2.5f : moveAcceleration;

            if (_HorizontalVelocity.CurrentVelocity > targetVelocity)
            {
                _HorizontalVelocity.CurrentVelocity -= acceleration * fixedUpdateInterval;
                if (_HorizontalVelocity.CurrentVelocity < targetVelocity)
                {
                    _HorizontalVelocity.CurrentVelocity = targetVelocity;
                }
            }
            else if (_HorizontalVelocity.CurrentVelocity < targetVelocity)
            {
                _HorizontalVelocity.CurrentVelocity += acceleration * fixedUpdateInterval;
                if (_HorizontalVelocity.CurrentVelocity > targetVelocity)
                {
                    _HorizontalVelocity.CurrentVelocity = targetVelocity;
                }
            }
        }
        else
        {
            if (_HorizontalVelocity.CurrentVelocity > 0)
            {
                _HorizontalVelocity.CurrentVelocity -= moveDeceleration * fixedUpdateInterval;
                if (_HorizontalVelocity.CurrentVelocity < 0)
                {
                    _HorizontalVelocity.CurrentVelocity = 0;
                }
            }
            else if (_HorizontalVelocity.CurrentVelocity < 0)
            {
                _HorizontalVelocity.CurrentVelocity += moveDeceleration * fixedUpdateInterval;
                if (_HorizontalVelocity.CurrentVelocity > 0)
                {
                    _HorizontalVelocity.CurrentVelocity = 0;
                }
            }
        }

        if (MathF.Abs(_HorizontalVelocity.CurrentVelocity) > 0.01f)
        {
            MoveHorizontally(_HorizontalVelocity.CurrentVelocity * fixedUpdateInterval, spritePool);
        }
    }

    private void Jump(float jumpVelocity, IAudioManager audioManager)
    {
        if (!_IsGrounded)
        {
            return;
        }

        _IsGrounded = false;
        audioManager.PlaySound("Jump");
        _VerticalVelocity.CurrentVelocity = -jumpVelocity;
    }

    private void MoveHorizontally(float amount, List<ISprite2D> spritePool)
    {
        float x = Position.X + amount;
        RectCollider futureCollider = new(new(x, Position.Y), Scale, Center);

        foreach (ISprite2D sprite in spritePool)
        {
            if (sprite.Collider.IsCollidingWith(futureCollider))
            {
                CollisionDirection dir = Collision.GetCollisionDirection(futureCollider, sprite.Collider);
                if (dir == CollisionDirection.Left && _HorizontalVelocity.CurrentVelocity <= 0f)
                {
                    x = sprite.Collider.Right;
                    _HorizontalVelocity.CurrentVelocity = 0f;
                    break;
                }
                if (dir == CollisionDirection.Right && _HorizontalVelocity.CurrentVelocity >= 0f)
                {
                    x = sprite.Collider.Left - Scale.X;
                    _HorizontalVelocity.CurrentVelocity = 0f;
                    break;
                }
            }
        }

        Position = new(x, Position.Y);
        Origin = Center;
    }

    private void MoveVertically(float amount, List<ISprite2D> spritePool)
    {
        float y = Position.Y + amount;
        RectCollider futureCollider = new(new(Position.X, y), Scale, Center);
        bool isGrounded = false;

        foreach (ISprite2D sprite in spritePool)
        {
            if (sprite.Collider.IsCollidingWith(futureCollider))
            {
                CollisionDirection dir = Collision.GetCollisionDirection(futureCollider, sprite.Collider);
                if (dir == CollisionDirection.Top && _VerticalVelocity.CurrentVelocity <= 0f)
                {
                    y = sprite.Collider.Bottom;
                    _VerticalVelocity.CurrentVelocity = 0f;
                    break;
                }
                if (dir == CollisionDirection.Bottom && _VerticalVelocity.CurrentVelocity >= 0f)
                {
                    y = sprite.Collider.Top - Scale.Y + 1;
                    _VerticalVelocity.CurrentVelocity = 0f;
                    isGrounded = true;
                    break;
                }
            }
        }

        _IsGrounded = isGrounded;
        Position = new(Position.X, y);
        Origin = Center;
    }

    private void CorrectSmallOverlaps(List<ISprite2D> spritePool)
    {
        RectCollider currentCollider = new(Position, Scale, Center);
        const float epsilon = 0.1f;

        foreach (var sprite in spritePool)
        {
            if (!sprite.Collider.IsCollidingWith(currentCollider))
            {
                continue;
            }

            CollisionDirection dir = Collision.GetCollisionDirection(currentCollider, sprite.Collider);

            switch (dir)
            {
                case CollisionDirection.Top:
                    Position = new(Position.X, sprite.Collider.Bottom + epsilon);
                    _VerticalVelocity.CurrentVelocity = 0f;
                    break;

                case CollisionDirection.Bottom:
                    Position = new(Position.X, sprite.Collider.Top - Scale.Y - epsilon);
                    _VerticalVelocity.CurrentVelocity = 0f;
                    _IsGrounded = true;
                    break;

                case CollisionDirection.Left:
                    Position = new(sprite.Collider.Right + epsilon, Position.Y);
                    _HorizontalVelocity.CurrentVelocity = 0f;
                    break;

                case CollisionDirection.Right:
                    Position = new(sprite.Collider.Left - Scale.X - epsilon, Position.Y);
                    _HorizontalVelocity.CurrentVelocity = 0f;
                    break;
            }

            currentCollider = new(Position, Scale, Center);
        }

        Origin = Center;
    }
}