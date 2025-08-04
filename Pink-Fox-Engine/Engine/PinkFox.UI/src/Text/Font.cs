using System.Runtime.InteropServices;
using SDL3;

namespace PinkFox.UI.Text;

public class Font : IDisposable
{
    private nint _Font;
    private SDL.Color _Color = new()
    {
        R = 255,
        G = 255,
        B = 255,
        A = 255
    };
    private bool _Disposed;

    public Font(nint font)
    {
        _Font = font;
    }

    public Font(string filePath, float fontSize)
    {
        _Font = TTF.OpenFont(filePath, fontSize);
        if (_Font == nint.Zero)
        {
            throw new Exception($"Failed to load font from '{filePath}': {SDL.GetError()}");
        }
    }

    public void DrawText(nint renderer, string text, float x = 0f, float y = 0f)
    {
        nint surface = TTF.RenderTextSolid(_Font, text, (nuint)text.Length, _Color);
        if (surface == nint.Zero)
        {
            Console.WriteLine($"Failed to render text surface: {SDL.GetError()}");
            return;
        }

        nint texture = SDL.CreateTextureFromSurface(renderer, surface);
        if (texture == nint.Zero)
        {
            Console.WriteLine($"Failed to create texture from surface: {SDL.GetError()}");
            SDL.Free(surface);
            return;
        }

        (int width, int height) = GetWidthAndHeight(surface);

        SDL.FRect destinationRect = new()
        {
            X = x,
            Y = y,
            W = width,
            H = height
        };

        SDL.RenderTexture(renderer, texture, nint.Zero, destinationRect);

        SDL.DestroyTexture(texture);
        SDL.Free(surface);
    }

    public void SetColor(SDL.Color color)
    {
        _Color = color;
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
            if (_Font != nint.Zero)
            {
                TTF.CloseFont(_Font);
                _Font = nint.Zero;
            }
        }

        _Disposed = true;
    }
}