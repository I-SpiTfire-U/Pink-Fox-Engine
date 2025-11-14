using PinkFox.Core.Debugging;
using PinkFox.Core.Types;
using SDL;

namespace PinkFox.Graphics.Rendering;

public class Texture2D : IDisposable
{
    public unsafe SDL_Texture* TextureHandle { get; init; }
    public float Width { get; init; }
    public float Height { get; init; }

    private bool _Disposed;

    public static unsafe Texture2D FromResource(string resourceName, Renderer renderer)
    {
        SDL_Surface* surface = Core.ResourceManager.CreateSurfaceFromResource(resourceName);
        return new Texture2D(surface, renderer);
    }

    public static unsafe Texture2D FromFile(string fileName, Renderer renderer)
    {
        SDL_Surface* surface = SDL3_image.IMG_Load(fileName);
        return new Texture2D(surface, renderer);
    }

    public unsafe Texture2D(SDL_Surface* surface, Renderer renderer)
    {
        ThrowIfSurfaceInvalid(surface);

        Width = surface->w;
        Height = surface->h;
        TextureHandle = CreateTexture(renderer, surface);
        SDL3.SDL_SetTextureBlendMode(TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);

        SDL3.SDL_DestroySurface(surface);
    }

    public unsafe Texture2D(Renderer renderer, int width, int height)
    {
        TextureHandle = SDL3.SDL_CreateTexture(renderer, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height);

        if (TextureHandle is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to create render target");
            throw new Exception(SDL3.SDL_GetError());
        }

        Width = width;
        Height = height;

        SDL3.SDL_SetTextureBlendMode(TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);
    }

    public unsafe void SetAsRenderTarget(Renderer renderer)
    {
        SDL3.SDL_SetRenderTarget(renderer, TextureHandle);
    }

    public unsafe void Render(Renderer renderer, FRect? sourceRect = null, FRect? destinationRect = null, double angle = 0d, SDL_FPoint? centerPoint = null, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE)
    {
        SDL_FRect* dstPtr = null;
        SDL_FRect* srcPtr = null;
        SDL_FPoint* ctrPtr = null;

        if (destinationRect.HasValue)
        {
            SDL_FRect dstVal = destinationRect.Value;
            dstPtr = &dstVal;
        }

        if (sourceRect.HasValue)
        {
            SDL_FRect srcVal = sourceRect.Value;
            srcPtr = &srcVal;
        }

        if (centerPoint.HasValue)
        {
            SDL_FPoint ctrVal = centerPoint.Value;
            ctrPtr = &ctrVal;
        }

        SDL3.SDL_RenderTextureRotated(renderer, TextureHandle, srcPtr, dstPtr, angle, ctrPtr, flipMode);
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

    private static unsafe SDL_Surface* CreateSurface(string path)
    {
        SDL_Surface* surface = SDL3_image.IMG_Load(path);
        ThrowIfSurfaceInvalid(surface);
        return surface;
    }

    private static unsafe void ThrowIfSurfaceInvalid(SDL_Surface* surface)
    {
        if (surface is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to load surface");
            throw new Exception(SDL3.SDL_GetError());
        }
    }

    private static unsafe void ThrowIfTextureInvalid(SDL_Texture* texture)
    {
        if (texture is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to create texture");
            throw new Exception(SDL3.SDL_GetError());
        }
    }
}