using System.Numerics;

namespace PinkFox.Core.Components;

public interface ICamera2D
{
    float Zoom { get; }
    Vector2 Position { get; }
    int ViewWidth { get; }
    int ViewHeight { get; }
    Vector2 ViewOffset { get; }

    void UpdatePosition(float dx, float dy);
    void SetPosition(Vector2 position);
    void SetZoom(float value);
    void UpdateZoom(float amount);
    void SetViewSize(int width, int height);
    
    Vector2 WorldToScreen(Vector2 worldPos);
    Vector2 ScreenToWorld(Vector2 screenPos);
    Matrix3x2 GetTransform();
}