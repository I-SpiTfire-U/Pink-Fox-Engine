using System.Numerics;
using System.Runtime.InteropServices;
using SDL;

namespace PinkFox.Graphics.Rendering;

public class Texture2D : IDisposable
{
    public unsafe SDL_Texture* TextureHandle { get; private set; }
    public float Width { get; init; }
    public float Height { get; init; }

    public unsafe bool IsValid => TextureHandle is not null && !_Disposed;
    private bool _Disposed;

    public unsafe Texture2D(string filePath, SDL_Renderer* renderer)
    {
        SDL_Surface* surface = LoadImage(filePath);
        (Width, Height) = GetWidthAndHeight(surface);
        TextureHandle = CreateTexture(renderer, surface);
        SDL3.SDL_DestroySurface(surface);
        SDL3.SDL_SetTextureBlendMode(TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);
    }

    public unsafe Texture2D(SDL_Surface* surface, SDL_Renderer* renderer)
    {
        ThrowIfSurfaceInvalid(surface);
        (Width, Height) = GetWidthAndHeight(surface);
        TextureHandle = CreateTexture(renderer, surface);
        SDL3.SDL_DestroySurface(surface);
        SDL3.SDL_SetTextureBlendMode(TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);
    }

    public unsafe void Draw(SDL_Renderer* renderer, Vector2 position)
    {
        SDL_FRect destinationRect = new()
        {
            x = position.X,
            y = position.Y,
            w = Width,
            h = Height
        };

        SDL3.SDL_RenderTexture(renderer, TextureHandle, null, &destinationRect);
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
            unsafe
            {
                if (TextureHandle is not null)
                {
                    SDL3.SDL_DestroyTexture(TextureHandle);
                    TextureHandle = null;
                }
            }
        }

        _Disposed = true;
    }

    ~Texture2D()
    {
        Dispose(false);
    }

    private static unsafe SDL_Texture* CreateTexture(SDL_Renderer* renderer, SDL_Surface* surface)
    {
        SDL_Texture* texture = SDL3.SDL_CreateTextureFromSurface(renderer, surface);
        ThrowIfTextureInvalid(texture);
        return texture;
    }

    private static unsafe SDL_Surface* LoadImage(string path)
    {
        SDL_Surface* surface = SDL3_image.IMG_Load(path);
        ThrowIfSurfaceInvalid(surface);
        return surface;
    }

    private static unsafe void ThrowIfSurfaceInvalid(SDL_Surface* surface)
    {
        if (surface is null)
        {
            throw new Exception($"Failed to load surface: {SDL3.SDL_GetError()}");
        }
    }

    private static unsafe void ThrowIfTextureInvalid(SDL_Texture* texture)
    {
        if (texture is null)
        {
            throw new Exception($"Failed to create texture: {SDL3.SDL_GetError()}");
        }
    }

    private static unsafe (int width, int height) GetWidthAndHeight(SDL_Surface* surface)
    {
        ThrowIfSurfaceInvalid(surface);
        return (surface->w, surface->h);
    }

    public static unsafe Texture2D FromPixels(SDL_Renderer* renderer, nint pixels, int width, int height, SDL_PixelFormat format)
    {
        int pitch = width * SDL3.SDL_BYTESPERPIXEL(format);
        SDL_Surface* surface = SDL3.SDL_CreateSurfaceFrom(width, height, format, pixels, pitch);
        if (surface is null)
        {
            throw new Exception($"Failed to create surface from pixels: {SDL3.SDL_GetError()}");
        }
        return new Texture2D(surface, renderer);
    }

    public static unsafe Texture2D CreateSquareTexture(SDL_Renderer* renderer, int width, int height, SDL_Color color)
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

                        ptr[offset + 0] = color.a;
                        ptr[offset + 1] = color.b;
                        ptr[offset + 2] = color.g;
                        ptr[offset + 3] = color.r;
                    }
                }
            }

            return FromPixels(renderer, pixels, width, height, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888);
        }
        finally
        {
            Marshal.FreeHGlobal(pixels);
        }
    }

    public static unsafe Texture2D CreateCircleTexture(SDL_Renderer* renderer, int width, int height, SDL_Color color)
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

                        ptr[offset + 0] = insideCircle ? color.a : (byte)0;
                        ptr[offset + 1] = insideCircle ? color.b : (byte)0;
                        ptr[offset + 2] = insideCircle ? color.g : (byte)0;
                        ptr[offset + 3] = insideCircle ? color.r : (byte)0;
                    }
                }
            }

            return FromPixels(renderer, pixels, width, height, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888);
        }
        finally
        {
            Marshal.FreeHGlobal(pixels);
        }
    }

    public static unsafe Texture2D CreateSquareOutlineTexture(SDL_Renderer* renderer, int width, int height, int outlineThickness, SDL_Color outlineColor)
    {
        int bpp = 4;
        int pitch = width * bpp;
        nint pixels = Marshal.AllocHGlobal(height * pitch);

        try
        {
            byte* ptr = (byte*)pixels.ToPointer();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = (y * pitch) + (x * bpp);

                    bool isOutline = x < outlineThickness || x >= width - outlineThickness ||
                                     y < outlineThickness || y >= height - outlineThickness;

                    if (isOutline)
                    {
                        ptr[offset + 0] = outlineColor.a;
                        ptr[offset + 1] = outlineColor.b;
                        ptr[offset + 2] = outlineColor.g;
                        ptr[offset + 3] = outlineColor.r;
                    }
                    else
                    {
                        ptr[offset + 0] = 0;
                        ptr[offset + 1] = 0;
                        ptr[offset + 2] = 0;
                        ptr[offset + 3] = 0;
                    }
                }
            }

            return FromPixels(renderer, pixels, width, height, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888);
        }
        finally
        {
            Marshal.FreeHGlobal(pixels);
        }
    }

    public static unsafe Texture2D CreateCircleOutlineTexture(SDL_Renderer* renderer, int width, int height, int outlineThickness, SDL_Color outlineColor)
    {
        int bpp = 4;
        int pitch = width * bpp;
        float centerX = width / 2f;
        float centerY = height / 2f;
        float radius = Math.Min(width, height) / 2f;

        float outerRadius = radius;
        float innerRadius = radius - outlineThickness;

        nint pixels = Marshal.AllocHGlobal(height * pitch);

        try
        {
            byte* ptr = (byte*)pixels.ToPointer();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx = x - centerX;
                    float dy = y - centerY;
                    float distSquared = dx * dx + dy * dy;

                    float outerR2 = outerRadius * outerRadius;
                    float innerR2 = innerRadius * innerRadius;

                    int offset = (y * pitch) + (x * bpp);

                    if (distSquared <= outerR2 && distSquared >= innerR2)
                    {
                        ptr[offset + 0] = outlineColor.a;
                        ptr[offset + 1] = outlineColor.b;
                        ptr[offset + 2] = outlineColor.g;
                        ptr[offset + 3] = outlineColor.r;
                    }
                    else
                    {
                        ptr[offset + 0] = 0;
                        ptr[offset + 1] = 0;
                        ptr[offset + 2] = 0;
                        ptr[offset + 3] = 0;
                    }
                }
            }

            return FromPixels(renderer, pixels, width, height, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888);
        }
        finally
        {
            Marshal.FreeHGlobal(pixels);
        }
    }
}