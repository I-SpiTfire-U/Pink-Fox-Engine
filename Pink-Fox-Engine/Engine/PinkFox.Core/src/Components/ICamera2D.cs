using System.Numerics;

namespace PinkFox.Core.Components;

public interface ICamera2D
{
    Vector2 Position { get; set; }
    float Zoom { get; set; }
    int ViewWidth { get; }
    int ViewHeight { get; }

    void Move(float dx, float dy);
    void SetViewSize(int width, int height);
    void ChangeZoom(float amount);
    void SetPosition(Vector2 position);
    Matrix3x2 GetTransform();
    Vector2 WorldToScreen(Vector2 worldPos);
    Vector2 ScreenToWorld(Vector2 screenPos);
}