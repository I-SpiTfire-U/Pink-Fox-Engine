using PinkFox.Core.Components;
using PinkFox.Core.Collisions;
using PinkFox.Graphics.Rendering;
using SDL3;
using System.Numerics;
using PinkFox.Core.Scenes;

namespace PinkFox.SampleGame;

public class PlayerObject : AnimatedSprite2D, ISprite2D
{
    private readonly Velocity _HorizontalVelocity;
    private readonly Velocity _VerticalVelocity;
    private readonly float _JumpForce;

    private bool _IsGrounded = false;

    public PlayerObject(string name, Velocity horizontalVelocity, Velocity verticalVelocity, float jumpForce, Texture2D texture, Dictionary<string, Animation> animations, Vector2 position, Vector2? origin = null, Vector2? scale = null, double rotation = 0f, SDL.FlipMode flipMode = SDL.FlipMode.None, bool isVisible = true)
    : base(name, texture, animations, position, origin, scale, rotation, flipMode, isVisible)
    {
        _HorizontalVelocity = horizontalVelocity;
        _VerticalVelocity = verticalVelocity;
        _JumpForce = jumpForce;
    }

    private Vector2 _PreviousPosition;

    private bool _JumpRequested = false;
    private float _MoveDirection = 0f;
    public void Update(float deltaTime, IInputManager inputManager)
    {
        _PreviousPosition = Position;
        
        if (inputManager.Keyboard.IsKeyDown(SDL.Keycode.Space) || (inputManager.Gamepads.AtIndex(0)?.IsButtonDown(SDL.GamepadButton.South) ?? false))
        {
            _JumpRequested = true;
        }

        _MoveDirection = 0f;
        if (inputManager.Keyboard.IsKeyHeld(SDL.Keycode.A) || inputManager.Gamepads.AtIndex(0)?.GetAxisFiltered(SDL.GamepadAxis.LeftX) < 0)
        {
            _MoveDirection = -1f;
        }
        if (inputManager.Keyboard.IsKeyHeld(SDL.Keycode.D) || inputManager.Gamepads.AtIndex(0)?.GetAxisFiltered(SDL.GamepadAxis.LeftX) > 0)
        {
            _MoveDirection = 1f;
        }
    }

    public void FixedUpdate(float fixedUpdateInterval, List<ISprite2D> spritePool, IAudioManager audioManager)
    {   
        float moveAcceleration = _HorizontalVelocity.AccelerationRate;
        float moveDeceleration = 1000f;

        float targetVelocity = 0f;

        _VerticalVelocity.CurrentVelocity += _VerticalVelocity.AccelerationRate * fixedUpdateInterval;

        if (_JumpRequested)
        {
            Jump(_JumpForce, audioManager);
            _JumpRequested = false;
        }

        MoveVertically(_VerticalVelocity.CurrentVelocity * fixedUpdateInterval, spritePool);

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

        if (_IsGrounded)
        {
            SetCurrentFrame(_HorizontalVelocity.CurrentVelocity >= _HorizontalVelocity.MaximumVelocity || _HorizontalVelocity.CurrentVelocity <= -_HorizontalVelocity.MaximumVelocity ? 2 : 0);
            return;
        }
        SetCurrentFrame(1);
    }

    public void Draw(nint renderer, ICamera2D? camera2D = null, float alpha = 1f)
    {
        Vector2 interpolatedPosition = Vector2.Lerp(_PreviousPosition, Position, alpha);
        Vector2 originalPosition = Position;
        Position = interpolatedPosition;
        base.Draw(renderer, camera2D);
        Position = originalPosition;
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

        foreach (ISprite2D sprite in spritePool)
        {
            if (Collider.IsCollidingWith(sprite.Collider))
            {
                CollisionDirection dir = Collision.GetCollisionDirection(Collider, sprite.Collider);
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
        bool isGrounded = false;

        foreach (ISprite2D sprite in spritePool)
        {
            if (Collider.IsCollidingWith(sprite.Collider))
            {
                CollisionDirection dir = Collision.GetCollisionDirection(Collider, sprite.Collider);
                if (dir == CollisionDirection.Bottom && _VerticalVelocity.CurrentVelocity >= -0.1f)
                {
                    y = sprite.Collider.Top - Scale.Y + 1;
                    _VerticalVelocity.CurrentVelocity = 0f;
                    isGrounded = true;
                    break;
                }
                if (dir == CollisionDirection.Top && _VerticalVelocity.CurrentVelocity <= 0.1f)
                {
                    y = sprite.Collider.Bottom;
                    _VerticalVelocity.CurrentVelocity = 0f;
                    break;
                }
            }
        }

        _IsGrounded = isGrounded;
        Position = new(Position.X, y);
        Origin = Center;
    }
}