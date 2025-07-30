using SDL3;

namespace PinkFox.Graphics.Basic;

public class Image2D : IDisposable
{
    public SDL.FRect DestinationRect { get; set; }
    public SDL.FRect? SourceRect { get; set; }
    private bool _Disposed = false;
    private nint _Texture;

    public Image2D(int x, int y, string imagePath, nint renderer, float scale = 1.0f)
    {
        nint surface = CreateSurface(imagePath);
        (int width, int height) = GetWidthAndHeight(surface);
        _Texture = CreateTexture(surface, renderer);
        SDL.DestroySurface(surface);

        DestinationRect = new()
        {
            X = x,
            Y = y,
            W = width * scale,
            H = height * scale
        };

        SourceRect = null;
    }

    public Image2D(int x, int y, SDL.FRect sourceRect, string imagePath, nint renderer, float scale = 1.0f)
    {
        nint surface = CreateSurface(imagePath);
        (int width, int height) = GetWidthAndHeight(surface);
        _Texture = CreateTexture(surface, renderer);
        SDL.DestroySurface(surface);

        DestinationRect = new()
        {
            X = x,
            Y = y,
            W = width * scale,
            H = height * scale
        };

        SourceRect = sourceRect;
    }

    public Image2D(SDL.FRect destinationRect, string imagePath, nint renderer)
    {
        nint surface = CreateSurface(imagePath);
        _Texture = CreateTexture(surface, renderer);
        SDL.DestroySurface(surface);

        DestinationRect = destinationRect;
        SourceRect = null;
    }

    public Image2D(SDL.FRect destinationRect, SDL.FRect sourceRect, string imagePath, nint renderer)
    {
        nint surface = CreateSurface(imagePath);
        _Texture = CreateTexture(surface, renderer);
        SDL.DestroySurface(surface);

        DestinationRect = destinationRect;
        SourceRect = sourceRect;
    }

    private static nint CreateSurface(string imagePath)
    {
        nint surface = Image.Load(imagePath);
        if (surface != IntPtr.Zero)
        {
            return surface;
        }
        throw new Exception($"Failed to load image: {SDL.GetError()}");
    }

    private static nint CreateTexture(nint surface, nint renderer)
    {
        nint texture = SDL.CreateTextureFromSurface(renderer, surface);
        if (texture == IntPtr.Zero)
        {
            throw new Exception($"Failed to create texture: {SDL.GetError()}");
        }
        return texture;
    }

    private static (int width, int height) GetWidthAndHeight(nint surface)
    {
        int width;
        int height;
        unsafe
        {
            SDL.Surface* surf = (SDL.Surface*)surface;
            width = surf->Width;
            height = surf->Height;
        }

        return (width, height);
    }

    public void Draw(nint renderer)
    {
        ObjectDisposedException.ThrowIf(_Disposed, nameof(Image2D));

        var destinationRect = DestinationRect;
        if (!SourceRect.HasValue)
        {
            SDL.RenderTexture(renderer, _Texture, IntPtr.Zero, in destinationRect);
            return;
        }
        var sourceRect = SourceRect.Value;
        SDL.RenderTexture(renderer, _Texture, in sourceRect, in destinationRect);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_Disposed)
        {
            if (_Texture != IntPtr.Zero)
            {
                SDL.DestroyTexture(_Texture);
                _Texture = IntPtr.Zero;
            }
            _Disposed = true;
        }
    }

    ~Image2D()
    {
        Dispose(false);
    }
}