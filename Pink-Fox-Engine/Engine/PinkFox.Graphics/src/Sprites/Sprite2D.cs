using System.Numerics;
using SDL;
using PinkFox.Graphics.Rendering;
using PinkFox.Core.Modules.Graphics;
using PinkFox.Core.Types;

namespace PinkFox.Graphics.Sprites;

public class Sprite2D : ISprite2D, IDisposable
{
    public string Name { get; init; }
    public Texture2D Texture { get; protected set; }
    public Vector2 TextureSize { get; protected set; }
    public FRect SourceRect { get; set; }
    
    public Vector2 Position { get; protected set; }
    public Vector2 Scale { get; protected set; }
    public double Rotation { get; protected set; }
    public SDL_FlipMode FlipMode { get; set; }
    public int Layer { get; set; }
    public bool IsVisible { get; set; }

    private bool _Disposed = false;
    public double RotationDegrees => Rotation * (180.0 / Math.PI);

    public Sprite2D(string name, Texture2D texture, Vector2 position, FRect? sourceRect = null, Vector2? scale = null, double rotation = 0.0d, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE, int layer = 0, bool isVisible = true)
    {
        Name = name;
        Texture = texture;
        TextureSize = new(Texture.Width, Texture.Height);

        SourceRect = sourceRect ?? new(0, 0, TextureSize.X, TextureSize.Y);

        Position = position;
        Scale = scale ?? TextureSize;
        Rotation = rotation;

        FlipMode = flipMode;
        Layer = layer;
        IsVisible = isVisible;
    }

    public unsafe void Render(Renderer renderer, ICamera2D? camera2D = null)
    {
        if (!IsVisible)
        {
            return;
        }

        Vector2 topLeft = (camera2D?.WorldToScreen(Position) ?? Position) - Scale / 2f;
        float zoom = camera2D?.Zoom ?? 1f;

        FRect destinationRect = new(topLeft.X, topLeft.Y, Scale.X * zoom, Scale.Y * zoom);
        FPoint zoomedPosition = new(Position.X * zoom, Position.Y * zoom);

        Texture.Render(renderer, destinationRect, SourceRect, RotationDegrees, zoomedPosition, FlipMode);
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