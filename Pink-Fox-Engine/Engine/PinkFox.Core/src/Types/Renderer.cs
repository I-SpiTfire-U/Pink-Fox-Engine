using System.Runtime.CompilerServices;
using PinkFox.Core.Debugging;
using PinkFox.Core.Modules.Graphics;
using SDL;

namespace PinkFox.Core.Types;

public class Renderer : IDisposable
{
    public unsafe SDL_Renderer* SdlRenderer { get; private set; }
    public IVirtualRenderer? VirtualRenderer { get; private set; }
    public SDL_Color ClearColor;

    private bool _Disposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe implicit operator SDL_Renderer*(Renderer r)
        => r.SdlRenderer;

    public unsafe Renderer() { }

    public unsafe void Initialize(Window window)
    {
        CreateRenderer(window);
    }

    private unsafe void CreateRenderer(Window window)
    {
        SdlRenderer = SDL3.SDL_CreateRenderer(window.SdlWindow, (byte*)null);
        if (SdlRenderer is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to create SDL renderer");
            throw new Exception(SDL3.SDL_GetError());
        }
        Terminal.LogMessage(LogLevel.Success, $"SDL renderer created");
    }

    public unsafe void SetClearColor(byte r, byte g, byte b)
    {   
        ClearColor = new()
        {
            r = r,
            g = g,
            b = b,
            a = 255
        };

        if (VirtualRenderer is not null)
        {
            VirtualRenderer.ClearColor = ClearColor;
        }

        SDL3.SDL_SetRenderDrawColor(this, ClearColor.r, ClearColor.g, ClearColor.b, ClearColor.a);
    }

    public void BeginVirtualRenderer()
    {
        VirtualRenderer?.Begin(this);
    }

    public void EndVirtualRenderer()
    {
        VirtualRenderer?.End(this);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private unsafe void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }

        if (disposing)
        {
            if (SdlRenderer is not null)
            {
                SDL3.SDL_DestroyRenderer(SdlRenderer);
                SdlRenderer = null;
            }
        }

        _Disposed = true;
    }
}