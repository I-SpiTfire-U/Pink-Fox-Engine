using System.Numerics;
using PinkFox.Core;
using PinkFox.Core.Collisions;
using PinkFox.Core.Components;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Rendering;

namespace PinkFox.Graphics.Cameras;

public class Camera2D : ICamera2D
{
    public float Zoom { get; private set; }
    public Vector2 Position { get; private set; }
    public int ViewWidth { get; private set; }
    public int ViewHeight { get; private set; }
    public Vector2 ViewOffset => new(ViewWidth / 2f / Zoom, ViewHeight / 2f / Zoom);

    private readonly float _MinimumZoom;
    private readonly float _MaximumZoom;

    public Camera2D(float zoom = 1f, float minZoom = 0.1f, float maxZoom = 5f)
    {
        Zoom = zoom;
        _MinimumZoom = minZoom;
        _MaximumZoom = maxZoom;
    }

    public Camera2D(Vector2 position, float zoom = 1f, float minZoom = 0.1f, float maxZoom = 5f)
    {
        Position = position;
        Zoom = zoom;
        _MinimumZoom = minZoom;
        _MaximumZoom = maxZoom;
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    public void UpdatePosition(float dx, float dy)
    {
        Position = new(Position.X + dx, Position.Y + dy);
    }

    public void SetZoom(float value)
    {
        Zoom = Math.Clamp(value, _MinimumZoom, _MaximumZoom);
    }

    public void UpdateZoom(float amount)
    {
        Zoom = Math.Clamp(Zoom + amount, _MinimumZoom, _MaximumZoom);
    }

    public void SetViewSize(int width, int height)
    {
        ViewWidth = width;
        ViewHeight = height;
    }

    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return (screenPos / Zoom) + Position;
    }

    public Vector2 WorldToScreen(Vector2 worldPos)
    {
        return (worldPos - Position) * Zoom;
    }

    public Matrix3x2 GetTransform()
    {
        return Matrix3x2.CreateTranslation(-Position) * Matrix3x2.CreateScale(Zoom) * Matrix3x2.CreateTranslation(ViewWidth / 2f, ViewHeight / 2f);
    }

    public RectCollider GetViewBounds()
    {
        float viewWidth = ViewWidth / Zoom;
        float viewHeight = ViewHeight / Zoom;

        return new RectCollider(Position, new Vector2(viewWidth, viewHeight));
    }

    public bool SpriteIsInView(ISprite2D sprite)
    {
        RectCollider spriteCollider = new(sprite.Position, sprite.Scale, sprite.Center);
        return GetViewBounds().IsCollidingWith(spriteCollider);
    }
}