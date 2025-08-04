using System.Numerics;
using System.Runtime.InteropServices;
using SDL3;

namespace PinkFox.Graphics.Rendering;

public class Texture2D : IDisposable
{
    public nint TextureHandle { get; private set; }
    public float Width { get; init; }
    public float Height { get; init; }

    public bool IsValid => TextureHandle != nint.Zero && !_Disposed;
    private bool _Disposed;

    public Texture2D(string filePath, nint renderer)
    {
        nint surface = LoadImage(filePath);
        (Width, Height) = GetWidthAndHeight(surface);
        TextureHandle = CreateTexture(surface, renderer);
        SDL.DestroySurface(surface);
        SDL.SetTextureBlendMode(TextureHandle, SDL.BlendMode.Blend);
    }

    public Texture2D(nint surface, nint renderer)
    {
        ThrowIfSurfaceInvalid(surface);
        (Width, Height) = GetWidthAndHeight(surface);
        TextureHandle = CreateTexture(surface, renderer);
        SDL.DestroySurface(surface);
        SDL.SetTextureBlendMode(TextureHandle, SDL.BlendMode.Blend);
    }

    public void Draw(nint renderer, Vector2 position)
    {
        SDL.FRect destinationRect = new()
        {
            X = position.X,
            Y = position.Y,
            W = Width,
            H = Height
        };

        SDL.RenderTexture(renderer, TextureHandle, nint.Zero, in destinationRect);
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

    private static nint CreateTexture(nint surface, nint renderer)
    {
        nint texture = SDL.CreateTextureFromSurface(renderer, surface);
        ThrowIfTextureInvalid(texture);
        return texture;
    }

    private static nint LoadImage(string path)
    {
        nint surface = Image.Load(path);
        ThrowIfSurfaceInvalid(surface);
        return surface;
    }

    private static void ThrowIfSurfaceInvalid(nint surface)
    {
        if (surface == nint.Zero)
        {
            throw new Exception($"Failed to load surface: {SDL.GetError()}");
        }
    }

    private static void ThrowIfTextureInvalid(nint texture)
    {
        if (texture == nint.Zero)
        {
            throw new Exception($"Failed to create texture: {SDL.GetError()}");
        }
    }

    private static (int width, int height) GetWidthAndHeight(nint surface)
    {
        ThrowIfSurfaceInvalid(surface);
        SDL.Surface surfaceStruct = Marshal.PtrToStructure<SDL.Surface>(surface);
        return (surfaceStruct.Width, surfaceStruct.Height);
    }

    public static Texture2D FromPixels(nint renderer, nint pixels, int width, int height, SDL.PixelFormat format)
    {
        int pitch = (int)(width * SDL.BytesPerPixel(format));
        nint surface = SDL.CreateSurfaceFrom(width, height, format, pixels, pitch);
        if (surface == nint.Zero)
        {
            throw new Exception($"Failed to create surface from pixels: {SDL.GetError()}");
        }
        return new Texture2D(surface, renderer);
    }

    public static Texture2D CreateSquareTexture(nint renderer, int width, int height, SDL.Color color)
    {
        int bpp = 4;
        int pitch = width * bpp;

        nint pixels = Marshal.AllocHGlobal(height * pitch);

        try
        {
            unsafe
            {
                byte* ptr = (byte*)pixels.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int offset = (y * pitch) + (x * bpp);

                        ptr[offset + 0] = color.R;
                        ptr[offset + 1] = color.G;
                        ptr[offset + 2] = color.B;
                        ptr[offset + 3] = color.A;
                    }
                }
            }

            return FromPixels(renderer, pixels, width, height, SDL.PixelFormat.RGBA8888);
        }
        finally
        {
            Marshal.FreeHGlobal(pixels);
        }
    }

    public static Texture2D CreateCircleTexture(nint renderer, int width, int height, SDL.Color color)
    {
        int bpp = 4;
        int pitch = width * bpp;
        float centerX = width / 2f;
        float centerY = height / 2f;
        float radius = Math.Min(width, height) / 2f;

        nint pixels = Marshal.AllocHGlobal(height * pitch);

        try
        {
            unsafe
            {
                byte* ptr = (byte*)pixels.ToPointer();
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        float dx = x - centerX;
                        float dy = y - centerY;
                        float distSquared = dx * dx + dy * dy;

                        bool insideCircle = distSquared <= radius * radius;

                        int offset = (y * pitch) + (x * bpp);

                        ptr[offset + 0] = insideCircle ? color.R : (byte)0;
                        ptr[offset + 1] = insideCircle ? color.G : (byte)0;
                        ptr[offset + 2] = insideCircle ? color.B : (byte)0;
                        ptr[offset + 3] = insideCircle ? color.A : (byte)0;
                    }
                }
            }

            return FromPixels(renderer, pixels, width, height, SDL.PixelFormat.RGBA8888);
        }
        finally
        {
            Marshal.FreeHGlobal(pixels);
        }
    }
}