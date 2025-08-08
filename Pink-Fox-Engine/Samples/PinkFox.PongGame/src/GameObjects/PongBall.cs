using System.Numerics;
using PinkFox.Core.Collisions;
using PinkFox.Core.Components;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Rendering;
using SDL;

namespace PongGame.GameObjects;

public class PongBall : Sprite2D, ISprite2D
{
    private float _BallSpeed = 150f;
    private float _HorizontalDirection = 1f;
    private float _VerticalDirection = 1f;
    private const float SpeedIncrease = 10f;

    public PongBall(string name, Texture2D texture, Vector2 position, Vector2 scale, bool isVisible = true)
    : base(name, texture, position, null, null, scale, 0d, SDL_FlipMode.SDL_FLIP_NONE, 0, isVisible) { }

    public void Update(float deltaTime, float windowHeight, IAudioManager audioManager)
    {
        float newXPosition = Position.X + _HorizontalDirection * _BallSpeed * deltaTime;
        float newYPosition = Position.Y + _VerticalDirection * _BallSpeed * deltaTime;

        if (Position.Y < 0 || Position.Y > windowHeight - Scale.Y)
        {
            audioManager.PlaySound("Collision");
            _VerticalDirection *= -1f;
            newYPosition = Math.Clamp(newYPosition, 0f, windowHeight - Scale.Y);
        }

        Position = new(newXPosition, newYPosition);
    }

    public void Reset(Vector2 position)
    {
        _BallSpeed = 150f;
        Position = position;
    }

    public void OnCollisionWithPaddle(PlayerPaddle paddle, IAudioManager audioManager)
    {
        if (!Collider.IsCollidingWith(paddle.Collider))
        {
            return;
        }

        audioManager.PlaySound("Collision");

        _BallSpeed += SpeedIncrease;
        _HorizontalDirection *= -1f;

        CollisionDirection collisionDirection = Collision.GetCollisionDirection(Collider, paddle.Collider);
        float newXPosition = collisionDirection == CollisionDirection.Left ? paddle.Collider.Right : paddle.Collider.Left - Scale.X;
        Position = new(newXPosition, Position.Y);
    }
}