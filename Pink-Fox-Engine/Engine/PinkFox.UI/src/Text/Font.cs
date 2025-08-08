using SDL;

namespace PinkFox.UI.Text;

public class Font : IDisposable
{
    public unsafe TTF_Font* Handle { get; private set; }

    private bool _Disposed;

    public unsafe Font(string filePath, float fontSize)
    {
        Handle = SDL3_ttf.TTF_OpenFont(filePath, fontSize);
        if (Handle is null)
        {
            throw new Exception($"Failed to load font from '{filePath}': {SDL3.SDL_GetError()}");
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
            unsafe
            {
                if (Handle is not null)
                {
                    SDL3_ttf.TTF_CloseFont(Handle);
                    Handle = null;
                }
            }
        }

        _Disposed = true;
    }
}