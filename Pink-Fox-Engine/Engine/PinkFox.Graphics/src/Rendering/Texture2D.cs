using System.Numerics;
using SDL;

namespace PinkFox.Graphics.Rendering;

public class Texture2D : IDisposable
{
    public unsafe SDL_Texture* TextureHandle { get; private set; }
    public float Width { get; init; }
    public float Height { get; init; }

    private bool _Disposed;

    public unsafe Texture2D(string resourceName, SDL_Renderer* renderer)
    {
        SDL_Surface* surface = Core.ResourceManager.CreateSurfaceFromResource(resourceName);
        Width = surface->w;
        Height = surface->h;
        TextureHandle = CreateTexture(renderer, surface);
        SDL3.SDL_DestroySurface(surface);
        SDL3.SDL_SetTextureBlendMode(TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);
    }

    public unsafe Texture2D(SDL_Surface* surface, SDL_Renderer* renderer)
    {
        ThrowIfSurfaceInvalid(surface);
        Width = surface->w;
        Height = surface->h;
        TextureHandle = CreateTexture(renderer, surface);
        SDL3.SDL_DestroySurface(surface);
        SDL3.SDL_SetTextureBlendMode(TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);
    }

    public unsafe Texture2D(SDL_Renderer* renderer, int width, int height)
    {
        TextureHandle = SDL3.SDL_CreateTexture(renderer, SDL_PixelFormat.SDL_PIXELFORMAT_RGBA8888, SDL_TextureAccess.SDL_TEXTUREACCESS_TARGET, width, height);

        if (TextureHandle is null)
        {
            throw new Exception($"Failed to create render target: {SDL3.SDL_GetError()}");
        }

        Width = width;
        Height = height;

        SDL3.SDL_SetTextureBlendMode(TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);
    }

    public unsafe void SetAsRenderTarget(SDL_Renderer* renderer)
    {
        SDL3.SDL_SetRenderTarget(renderer, TextureHandle);
    }

    public unsafe void Draw(SDL_Renderer* renderer, SDL_FRect destinationRect, SDL_FRect? sourceRect = null)
    {
        if (sourceRect is not null)
        {
            SDL_FRect sRect = sourceRect.Value;
            SDL3.SDL_RenderTexture(renderer, TextureHandle, &sRect, &destinationRect);
            return;
        }
        SDL3.SDL_RenderTexture(renderer, TextureHandle, null, &destinationRect);
    }

    public unsafe void Draw(SDL_Renderer* renderer, float xPosition, float yPosition, float scale = 1f, SDL_FRect? sourceRect = null)
    {
        float width = Width;
        float height = Height;

        if (sourceRect.HasValue)
        {
            width = sourceRect.Value.w;
            height = sourceRect.Value.h;
        }

        width *= scale;
        height *= scale;

        SDL_FRect destinationRect = new()
        {
            x = xPosition,
            y = yPosition,
            w = width,
            h = height
        };

        if (sourceRect is not null)
        {
            SDL_FRect sRect = sourceRect.Value;
            SDL3.SDL_RenderTexture(renderer, TextureHandle, &sRect, &destinationRect);
            return;
        }
        SDL3.SDL_RenderTexture(renderer, TextureHandle, null, &destinationRect);
    }

    public unsafe void Draw(SDL_Renderer* renderer, Vector2 position, float scale = 1f, SDL_FRect? sourceRect = null)
    {
        float width = Width;
        float height = Height;

        if (sourceRect.HasValue)
        {
            width = sourceRect.Value.w;
            height = sourceRect.Value.h;
        }

        width *= scale;
        height *= scale;

        SDL_FRect destinationRect = new()
        {
            x = position.X,
            y = position.Y,
            w = width,
            h = height
        };

        if (sourceRect is not null)
        {
            SDL_FRect sRect = sourceRect.Value;
            SDL3.SDL_RenderTexture(renderer, TextureHandle, &sRect, &destinationRect);
            return;
        }
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
}