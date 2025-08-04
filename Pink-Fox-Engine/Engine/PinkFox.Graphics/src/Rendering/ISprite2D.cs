using System.Numerics;
using PinkFox.Core.Collisions;
using PinkFox.Core.Components;
using PinkFox.Graphics.Rendering;
using SDL3;

namespace PinkFox.Core.Scenes;

public interface ISprite2D
{
    string Name { get; init; }
    Texture2D Texture { get; set; }
    Vector2 Position { get; set; }
    Vector2 Origin { get; set; }
    Vector2 Scale { get; set; }
    double Rotation { get; set; }
    SDL.FlipMode FlipMode { get; set; }
    bool IsVisible { get; set; }

    Vector2 TextureSize { get; }
    Vector2 Center { get; }
    RectCollider Collider { get; }

    void Draw(nint renderer, ICamera2D? camera2D = null);
    void Dispose();
}