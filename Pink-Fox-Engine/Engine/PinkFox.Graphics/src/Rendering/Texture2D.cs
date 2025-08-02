using SDL3;

namespace PinkFox.Graphics.Rendering;

public class Texture2D : IDisposable
{
    public nint TextureHandle { get; private set; }
    public int Width { get; init; }
    public int Height { get; init; }

    public bool IsDisposed => _Disposed;
    private bool _Disposed;

    public Texture2D(string filePath, nint renderer)
    {
        nint surface = LoadImage(filePath);
        (Width, Height) = GetWidthAndHeight(surface);
        TextureHandle = CreateTexture(surface, renderer);
        SDL.SetTextureBlendMode(TextureHandle, SDL.BlendMode.Blend);
        SDL.DestroySurface(surface);
    }

    public Texture2D(nint surface, nint renderer)
    {
        if (surface == nint.Zero)
        {
            throw new Exception($"Failed to load image: {SDL.GetError()}");
        }
        (Width, Height) = GetWidthAndHeight(surface);
        TextureHandle = CreateTexture(surface, renderer);
        SDL.SetTextureBlendMode(TextureHandle, SDL.BlendMode.Blend);
        SDL.DestroySurface(surface);
    }

    private static nint CreateTexture(nint surface, nint renderer)
    {
        nint texture = SDL.CreateTextureFromSurface(renderer, surface);
        if (texture != nint.Zero)
        {
            return texture;
        }
        throw new Exception($"Failed to create texture: {SDL.GetError()}");
    }

    private static nint LoadImage(string path)
    {
        nint surface = Image.Load(path);
        if (surface != nint.Zero)
        {
            return surface;
        }
        throw new FileNotFoundException($"Could not load image: {path}");
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

    public static Texture2D FromSurface(nint surface, nint renderer) => new(surface, renderer);

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
            if (TextureHandle != nint.Zero)
            {
                SDL.DestroyTexture(TextureHandle);
                TextureHandle = nint.Zero;
            }
        }

        _Disposed = true;
    }

    ~Texture2D()
    {
        Dispose(false);
    }
}