using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Collisions;
using SDL;
using PinkFox.Graphics.Rendering;

namespace PinkFox.Graphics.Sprites;

public class Sprite2D : ISprite2D, IDisposable
{
    public string Name { get; init; }
    public Texture2D Texture { get; protected set; }
    public Vector2 TextureSize { get; protected set; }
    public SDL_FRect SourceRect { get; set; }
    
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public double Rotation { get; set; }
    public SDL_FlipMode FlipMode { get; set; }
    public int Layer { get; set; }
    public bool IsVisible { get; set; }

    private bool _Disposed = false;

    public Vector2 Center => Position + Scale / 2f;
    public ICollider Collider => new BoxCollider(Position, Scale);
    public double RotationDegrees => Rotation * (180.0 / Math.PI);

    public Sprite2D(string name, Texture2D texture, Vector2 position, Vector2? origin = null, SDL_FRect? sourceRect = null, Vector2? scale = null, double rotation = 0.0d, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE, int layer = 0, bool isVisible = true)
    {
        Name = name;
        Texture = texture;
        TextureSize = new(Texture.Width, Texture.Height);

        SourceRect = sourceRect ?? new()
        {
            x = 0,
            y = 0,
            w = TextureSize.X,
            h = TextureSize.Y
        };
        Position = position;
        Scale = scale ?? TextureSize;
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

        Vector2 screenPosition = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;

        SDL_FRect destinationRect = new()
        {
            x = screenPosition.X,
            y = screenPosition.Y,
            w = Scale.X * zoom,
            h = Scale.Y * zoom
        };

        SDL_FRect sourceRect = SourceRect;

        SDL_FPoint center = new()
        {
            x = Origin.X * zoom,
            y = Origin.Y * zoom
        };

        SDL3.SDL_RenderTextureRotated(renderer, Texture.TextureHandle, &sourceRect, &destinationRect, RotationDegrees, &center, FlipMode);
    }

    public void SetNewTexture(Texture2D texture)
    {
        Texture = texture;
        TextureSize = new(Texture.Width, Texture.Height);
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

    public override string ToString() => $"{Name} Pos:{Position} Scale:{Scale} Rot:{RotationDegrees}° Visible:{IsVisible}";
}