using SDL3;
using PinkFox.Core.Physics;
using System.Numerics;
using PinkFox.Core.Interfaces;
using System.Runtime.InteropServices;

namespace PinkFox.Graphics.Rendering;

public class Sprite2D : IDisposable
{
    public Vector2 Position { get; set; }
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float Rotation { get; set; }
    public SDL.FlipMode FlipMode { get; set; }
    public bool IsVisible { get; set; }

    public Texture2D Texture;
    private SDL.FRect? _SourceRect;
    private bool _Disposed = false;

    public float RawWidth => _SourceRect?.W ?? Texture.Width;
    public float RawHeight => _SourceRect?.H ?? Texture.Height;
    public float Width => RawWidth * ScaleX;
    public float Height => RawHeight * ScaleY;
    public Vector2 Center => new(Position.X + Width / 2f, Position.Y + Height / 2f);
    public ICollider Collider => new RectCollider(Position.X, Position.Y, Width, Height);

    public Sprite2D(Texture2D texture, float x, float y, float scale = 1.0f, float rotation = 0.0f, SDL.FRect? sourceRect = null, SDL.FlipMode flipMode = SDL.FlipMode.None, bool isVisible = true)
    {
        Texture = texture;
        Position = new(x, y);
        ScaleX = scale;
        ScaleY = scale;
        Rotation = rotation;
        _SourceRect = sourceRect;
        FlipMode = flipMode;
        IsVisible = isVisible;
    }

    public Sprite2D(Texture2D texture, float x, float y, float scaleX = 1.0f, float scaleY = 1.0f, float rotation = 0.0f, SDL.FRect? sourceRect = null, SDL.FlipMode flipMode = SDL.FlipMode.None, bool isVisible = true)
    {
        Texture = texture;
        Position = new(x, y);
        ScaleX = scaleX;
        ScaleY = scaleY;
        Rotation = rotation;
        _SourceRect = sourceRect;
        FlipMode = flipMode;
        IsVisible = isVisible;
    }

    public void SetSourceRect(float x, float y, float width, float height)
    {
        _SourceRect = new()
        {
            X = x,
            Y = y,
            W = width,
            H = height
        };
    }

    public void SetSourceRect(SDL.FRect sourceRect)
    {
        _SourceRect = sourceRect;
    }

    public void ClearSourceRect()
    {
        _SourceRect = null;
    }

    private static (int width, int height) GetWidthAndHeight(nint surface)
    {
        if (surface == nint.Zero)
        {
            return (0, 0);
        }
        
        SDL.Surface surfaceStruct = Marshal.PtrToStructure<SDL.Surface>(surface);
        return (surfaceStruct.Width, surfaceStruct.Height);
    }

    public void Draw(nint renderer, ICamera2D? camera2D = null)
    {
        if (!IsVisible)
        {
            return;
        }

        Vector2 screenPos = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;

        SDL.FRect destinationRect = new()
        {
            X = screenPos.X,
            Y = screenPos.Y,
            W = Width * zoom,
            H = Height * zoom
        };

        unsafe
        {
            var center = stackalloc SDL.FPoint[1];
            center[0].X = RawWidth * ScaleX * zoom / 2f;
            center[0].Y = RawHeight * ScaleY * zoom / 2f;

            if (_SourceRect.HasValue)
            {
                SDL.RenderTextureRotated(renderer, Texture.TextureHandle, _SourceRect.Value, in destinationRect, Rotation, (nint)center, FlipMode);
                return;
            }
            SDL.RenderTextureRotated(renderer, Texture.TextureHandle, nint.Zero, in destinationRect, Rotation, (nint)center, FlipMode);
        }
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

    ~Sprite2D()
    {
        Dispose(false);
    }
}