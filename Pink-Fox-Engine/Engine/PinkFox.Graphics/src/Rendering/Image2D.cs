using System.Numerics;
using SDL3;

namespace PinkFox.Graphics.Rendering;

public class Image2D : IDisposable
{
    public Vector2 Position { get; set; }
    public float Scale { get; set; }
    public float Rotation { get; set; }
    public SDL.FlipMode FlipMode { get; set; }
    private readonly Texture2D _Texture;
    private bool _Disposed;

    public Image2D(Texture2D texture, float x, float y, float scale = 1.0f, float rotation = 0.0f, SDL.FlipMode flipMode = SDL.FlipMode.None)
    {
        _Texture = texture;
        Position = new(x, y);
        Scale = scale;
        Rotation = rotation;
        FlipMode = flipMode;
    }

    public Image2D(Texture2D texture, Vector2 position, float scale = 1.0f, float rotation = 0.0f, SDL.FlipMode flipMode = SDL.FlipMode.None)
    {
        _Texture = texture;
        Position = position;
        Scale = scale;
        Rotation = rotation;
        FlipMode = flipMode;
    }

    public float Width => _Texture.Width * Scale;
    public float Height => _Texture.Height * Scale;

    public void Draw(nint renderer)
    {
        var destinationRect = new SDL.FRect
        {
            X = Position.X,
            Y = Position.Y,
            W = _Texture.Width * Scale,
            H = _Texture.Height * Scale
        };

        unsafe
        {
            var center = stackalloc SDL.FPoint[1];
            center[0].X = Width / 2f;
            center[0].Y = Height / 2f;

            SDL.RenderTextureRotated(renderer, _Texture.TextureHandle, nint.Zero, in destinationRect, Rotation, (nint)center, FlipMode);
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

        if (disposing)
        {
            _Texture.Dispose();
        }

        _Disposed = true;
    }
}