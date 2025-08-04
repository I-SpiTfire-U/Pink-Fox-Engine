using System.Runtime.InteropServices;
using SDL;

namespace PinkFox.UI.Text;

public class Font : IDisposable
{
    private unsafe TTF_Font* _Font;
    private SDL_Color _Color = new()
    {
        r = 255,
        g = 255,
        b = 255,
        a = 255
    };
    private bool _Disposed;

    public unsafe Font(string filePath, float fontSize)
    {
        _Font = SDL3_ttf.TTF_OpenFont(filePath, fontSize);
        if (_Font is null)
        {
            throw new Exception($"Failed to load font from '{filePath}': {SDL3.SDL_GetError()}");
        }
    }

    public unsafe void DrawText(SDL_Renderer* renderer, string text, float x = 0f, float y = 0f)
    {
        SDL_Surface* surface = SDL3_ttf.TTF_RenderText_Solid(_Font, text, (nuint)text.Length, _Color);
        if (surface is null)
        {
            Console.WriteLine($"Failed to render text surface: {SDL3.SDL_GetError()}");
            return;
        }

        SDL_Texture* texture = SDL3.SDL_CreateTextureFromSurface(renderer, surface);
        if (texture is null)
        {
            Console.WriteLine($"Failed to create texture from surface: {SDL3.SDL_GetError()}");
            SDL3.SDL_free(surface);
            return;
        }

        (int width, int height) = GetWidthAndHeight(surface);

        SDL_FRect destinationRect = new()
        {
            x = x,
            y = y,
            w = width,
            h = height
        };

        SDL3.SDL_RenderTexture(renderer, texture, null, &destinationRect);

        SDL3.SDL_DestroyTexture(texture);
        SDL3.SDL_DestroySurface(surface);
    }

    public void SetColor(SDL_Color color)
    {
        _Color = color;
    }

    private static unsafe (int width, int height) GetWidthAndHeight(SDL_Surface* surface)
    {
        return surface is null ? (0, 0) : (surface->w, surface->h);
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
                if (_Font is not null)
                {
                    SDL3_ttf.TTF_CloseFont(_Font);
                    _Font = null;
                }
            }
        }

        _Disposed = true;
    }
}