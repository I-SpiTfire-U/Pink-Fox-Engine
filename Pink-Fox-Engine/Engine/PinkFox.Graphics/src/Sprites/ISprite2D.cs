using System.Numerics;
using PinkFox.Core.Collisions;
using PinkFox.Core.Components;
using PinkFox.Graphics.Rendering;
using SDL;

namespace PinkFox.Graphics.Sprites;

/// <summary>
/// Interface representing a 2D sprite with transform, texture, rendering, and collision properties.
/// </summary>
public interface ISprite2D : IDisposable
{
    /// <summary>Unique name of the sprite.</summary>
    string Name { get; init; }

    /// <summary>The texture used for rendering the sprite.</summary>
    Texture2D Texture { get; }

    /// <summary>Size of the texture.</summary>
    Vector2 TextureSize { get; }

    /// <summary>Optional source rectangle from the texture to render.</summary>
    SDL_FRect SourceRect { get; }

    /// <summary>Position of the sprite in world space.</summary>
    Vector2 Position { get; set; }

    /// <summary>Origin point used for rotation and positioning, relative to sprite bounds.</summary>
    Vector2 Origin { get; set; }

    /// <summary>Scale of the sprite.</summary>
    Vector2 Scale { get; set; }

    /// <summary>Rotation angle in radians.</summary>
    double Rotation { get; set; }

    /// <summary>Flipping mode for sprite rendering.</summary>
    SDL_FlipMode FlipMode { get; set; }

    /// <summary>Rendering layer / draw order.</summary>
    public int Layer { get; set; }

    /// <summary>Visibility flag indicating if the sprite should be drawn.</summary>
    bool IsVisible { get; set; }

    /// <summary>Center position of the sprite.</summary>
    Vector2 Center { get; }

    /// <summary>Bounding collider for the sprite.</summary>
    ICollider Collider { get; }

    /// <summary>Rotation angle in degrees.</summary>
    double RotationDegrees { get; }

    /// <summary>
    /// Draws the sprite using the given SDL renderer and optional camera.
    /// </summary>
    /// <param name="renderer">Pointer to the SDL renderer.</param>
    /// <param name="camera2D">Optional camera for coordinate transformations.</param>
    unsafe void Draw(SDL_Renderer* renderer, ICamera2D? camera2D = null);
}