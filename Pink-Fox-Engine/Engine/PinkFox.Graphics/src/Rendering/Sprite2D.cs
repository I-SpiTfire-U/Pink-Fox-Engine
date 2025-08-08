using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Scenes;
using PinkFox.Core.Collisions;
using SDL;

namespace PinkFox.Graphics.Rendering;

public class Sprite2D : ISprite2D, IDisposable
{
    public string Name { get; init; }
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public SDL_FRect? SourceRect { get; set; }
    public Vector2 Scale { get; set; }
    public double Rotation { get; set; }
    public SDL_FlipMode FlipMode { get; set; }
    public int Layer { get; set; }
    public bool IsVisible { get; set; }

    private bool _Disposed = false;

    public Vector2 TextureSize => new(Texture.Width, Texture.Height);
    public Vector2 Center => Position + Scale / 2f;
    public RectCollider Collider => new(Position, Scale);

    public Sprite2D(string name, Texture2D texture, Vector2 position, Vector2? origin = null, SDL_FRect? sourceRect = null, Vector2? scale = null, double rotation = 0.0f, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE, int layer = 0, bool isVisible = true)
    {
        Name = name;
        Texture = texture;
        Position = position;
        SourceRect = sourceRect;
        Scale = scale ?? new(Texture.Width, Texture.Height);
        Origin = origin ?? Center;
        Rotation = rotation;
        FlipMode = flipMode;
        Layer = layer;
        IsVisible = isVisible;
    }

    public unsafe void Draw(SDL_Renderer* renderer, ICamera2D? camera2D = null)
    {
        if (!IsVisible)
        {
            return;
        }

        Vector2 screenPos = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;

        SDL_FRect destinationRect = new()
        {
            x = screenPos.X,
            y = screenPos.Y,
            w = Scale.X * zoom,
            h = Scale.Y * zoom
        };

        SDL_FPoint center = new()
        {
            x = Scale.X * zoom / 2f,
            y = Scale.Y * zoom / 2f
        };

        if (SourceRect.HasValue)
        {
            SDL_FRect sourceRect = SourceRect.Value;
            SDL3.SDL_RenderTextureRotated(renderer, Texture.TextureHandle, &sourceRect, &destinationRect, Rotation, &center, FlipMode);
            return;
        }
        SDL3.SDL_RenderTextureRotated(renderer, Texture.TextureHandle, null, &destinationRect, Rotation, &center, FlipMode);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }

        if (disposing) { }

        _Disposed = true;
    }
}