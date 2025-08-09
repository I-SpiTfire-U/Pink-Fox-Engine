using System.Numerics;
using PinkFox.Core.Collisions;
using PinkFox.Core.Components;
using PinkFox.Graphics.Rendering;
using SDL;

namespace PinkFox.Graphics.Sprites;

public interface ISprite2D
{
    string Name { get; init; }
    Texture2D Texture { get; set; }
    Vector2 Position { get; set; }
    Vector2 Origin { get; set; }
    Vector2 Scale { get; set; }
    double Rotation { get; set; }
    SDL_FlipMode FlipMode { get; set; }
    public int Layer { get; set; }
    bool IsVisible { get; set; }

    Vector2 TextureSize { get; }
    Vector2 Center { get; }
    BoxCollider Collider { get; }

    unsafe void Draw(SDL_Renderer* renderer, ICamera2D? camera2D = null);
    void Dispose();
}