using SDL;

namespace PinkFox.UI.Text;

public class Font : IDisposable
{
    public unsafe TTF_Font* Handle { get; private set; }
    public unsafe TTF_TextEngine* Engine { get; private set; }

    private bool _Disposed;

    public unsafe Font(string resourceName, float fontSize, TextEngine textEngine)
    {
        Handle = Core.ResourceManager.CreateFontFromResource(resourceName, fontSize);
        if (Handle is null)
        {
            throw new Exception($"Failed to load font from '{resourceName}': {SDL3.SDL_GetError()}");
        }

        Engine = textEngine.Engine;
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