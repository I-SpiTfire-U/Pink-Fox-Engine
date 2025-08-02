using System.Numerics;
using PinkFox.Core.Interfaces;

namespace PinkFox.Graphics.Cameras;

public class Camera2D : ICamera2D
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; }
    private readonly float _MinZoom;
    private readonly float _MaxZoom;

    public int ViewWidth { get; private set; }
    public int ViewHeight { get; private set; }

    public Camera2D(float x, float y, float zoom = 1f, float minZoom = 0.1f, float maxZoom = 5f)
    {
        Position = new(x, y);
        Zoom = zoom;
        _MinZoom = minZoom;
        _MaxZoom = maxZoom;
    }

    public Camera2D(Vector2 position, float zoom = 1f, float minZoom = 0.1f, float maxZoom = 5f)
    {
        Position = position;
        Zoom = zoom;
        _MinZoom = minZoom;
        _MaxZoom = maxZoom;
    }

    public Camera2D(float zoom = 1f, float minZoom = 0.1f, float maxZoom = 5f)
    {
        Zoom = zoom;
        _MinZoom = minZoom;
        _MaxZoom = maxZoom;
    }

    public void Move(float dx, float dy)
    {
        Position = new(Position.X + dx, Position.Y + dy);
    }

    public void ChangeZoom(float amount)
    {
        Zoom = Math.Clamp(Zoom + amount, _MinZoom, _MaxZoom);
    }

    public void SetPosition(Vector2 position)
    {
        Position = position;
    }

    public void SetViewSize(int width, int height)
    {
        ViewWidth = width;
        ViewHeight = height;
    }

    public Matrix3x2 GetTransform()
    {
        return Matrix3x2.CreateTranslation(-Position) * Matrix3x2.CreateScale(Zoom) * Matrix3x2.CreateTranslation(ViewWidth / 2f, ViewHeight / 2f);
    }

    public Vector2 ScreenToWorld(Vector2 screenPos)
    {
        return (screenPos / Zoom) + Position;
    }

    public Vector2 WorldToScreen(Vector2 worldPos)
    {
        return (worldPos - Position) * Zoom;
    }
}