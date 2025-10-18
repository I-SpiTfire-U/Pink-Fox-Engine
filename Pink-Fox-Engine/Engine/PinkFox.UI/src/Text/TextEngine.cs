using SDL;

namespace PinkFox.UI.Text;

public class TextEngine : IDisposable
{
    public unsafe TTF_TextEngine* Engine { get; private set; }

    private bool _Disposed;

    public unsafe TextEngine(SDL_Renderer* renderer)
    {
        Engine = SDL3_ttf.TTF_CreateRendererTextEngine(renderer);
        if (Engine is null)
        {
            throw new Exception($"Failed to create text engine: {SDL3.SDL_GetError()}");
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
                if (Engine is not null)
                {
                    SDL3_ttf.TTF_DestroyRendererTextEngine(Engine);
                    Engine = null;
                }
            }
        }

        _Disposed = true;
    }
}