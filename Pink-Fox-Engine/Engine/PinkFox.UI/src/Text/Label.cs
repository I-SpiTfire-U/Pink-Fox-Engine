using System.Numerics;
using SDL;

namespace PinkFox.UI.Text;

public class Label : IDisposable
{
    public Vector2 Position { get; set; }
    public SDL_Color Color { get; set; } = new()
    {
        r = 255,
        g = 255,
        b = 255,
        a = 255
    };
    public string? Text { get; private set; }

    private int _Width;
    private int _Height;
    private unsafe SDL_Texture* _Texture;
    private readonly unsafe SDL_Renderer* _Renderer;
    private readonly Font _Font;

    private bool _Disposed;

    public unsafe Label(SDL_Renderer* renderer, Font font, string text, Vector2 position)
    {
        _Renderer = renderer;
        _Font = font;
        Position = position;
        SetText(text);
    }

    public unsafe void SetText(string text)
    {
        if (Text == text)
        {
            return;
        }

        Text = text;

        if (_Texture is not null)
        {
            SDL3.SDL_DestroyTexture(_Texture);
            _Texture = null;
        }

        SDL_Surface* surface = SDL3_ttf.TTF_RenderText_Solid(_Font.Handle, Text, (nuint)Text.Length, Color);
        if (surface is null)
        {
            return;
        }

        _Width = surface->w;
        _Height = surface->h;

        _Texture = SDL3.SDL_CreateTextureFromSurface(_Renderer, surface);
        SDL3.SDL_DestroySurface(surface);
    }

    public unsafe void Draw()
    {
        if (_Texture is null)
        {
            return;
        }

        SDL_FRect dest = new()
        {
            x = Position.X,
            y = Position.Y,
            w = _Width,
            h = _Height
        };

        SDL3.SDL_RenderTexture(_Renderer, _Texture, null, &dest);
    }

    public unsafe void Dispose()
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
                if (_Texture is not null)
                {
                    SDL3.SDL_DestroyTexture(_Texture);
                    _Texture = null;
                }
            }
        }

        _Disposed = true;
    }
}
