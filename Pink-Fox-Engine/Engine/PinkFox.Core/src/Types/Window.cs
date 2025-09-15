using System.Numerics;
using System.Runtime.CompilerServices;
using PinkFox.Core.Debugging;
using PinkFox.Core.Scenes;
using SDL;

namespace PinkFox.Core.Types;

public class Window : IDisposable
{
    public string Title { get; private set; } = "Pink Fox Game";
    public Vector2 Size { get; private set; } = new Vector2(800, 600);
    public Vector2 Center { get; private set; }

    public unsafe SDL_Window* SdlWindow { get; private set; }
    public SDL_WindowID WindowId { get; private set; }
    public WindowFlagManager FlagManager { get; private set; }

    public int Width => (int)Size.X;
    public int Height => (int)Size.Y;

    public Renderer? Renderer { get; private set; }

    public SceneManager Scenes { get; set; }

    private bool _Disposed;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static unsafe implicit operator SDL_Window*(Window w)
        => w.SdlWindow;

    public Window(SDL_WindowFlags windowFlags)
    {
        FlagManager = new WindowFlagManager(this, windowFlags);
        Scenes = new();
    }

    public unsafe void Initialize(int width, int height, string? title, string? iconResource)
    {
        SetTitle(title);
        SetSize(width, height);

        CreateWindow();
        if (!string.IsNullOrEmpty(iconResource))
        {
            Terminal.LogMessage(LogLevel.Success, $"Window icon set");
            SetIcon(iconResource);
        }
    }

    public void AttachRenderer(Renderer renderer)
    {
        Renderer = renderer;
        renderer.Initialize(this);
    }

    public void CreateAndAttachRenderer()
    {
        Renderer = new Renderer();
        Renderer.Initialize(this);
    }

    public void SetRenderClearColor(byte r, byte g, byte b)
    {
        if (Renderer is null)
        {
            Terminal.LogMessage(LogLevel.Error, "No renderer attached, cannot set clear color");
            return;
        }
        Renderer.SetClearColor(r, g, b);
    }

    private unsafe void CreateWindow()
    {
        SdlWindow = SDL3.SDL_CreateWindow(Title, Width, Height, FlagManager.Flags);
        WindowId = SDL3.SDL_GetWindowID(SdlWindow);

        if (SdlWindow is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to create SDL window");
            throw new Exception(SDL3.SDL_GetError());
        }
        Terminal.LogMessage(LogLevel.Success, $"SDL window created");
    }

    public unsafe void SetIcon(string resourceName)
    {
        SDL_Surface* surface = ResourceManager.CreateSurfaceFromResource(resourceName);
        if (surface is null)
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to load window icon");
            throw new Exception(SDL3.SDL_GetError());
        }

        SDL3.SDL_SetWindowIcon(SdlWindow, surface);
        SDL3.SDL_free(surface);
        Terminal.LogMessage(LogLevel.Success, $"Window icon set to resource '{resourceName}'");
    }

    public unsafe void SetSize(int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            Terminal.LogMessage(LogLevel.Error, "Window dimensions must be greater than zero");
            throw new Exception();
        }

        Size = new Vector2(width, height);
        Center = new Vector2(width / 2f, height / 2f);

        if (SdlWindow is not null)
        {
            SDL3.SDL_SetWindowSize(SdlWindow, width, height);
        }
    }

    public unsafe void SetTitle(string? title)
    {
        if (!string.IsNullOrEmpty(title))
        {
            Title = title;
        }

        if (SdlWindow is not null)
        {
            SDL3.SDL_SetWindowTitle(SdlWindow, Title);
        }
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
            if (SdlWindow is not null)
            {
                SDL3.SDL_DestroyWindow(SdlWindow);
                SdlWindow = null;
            }
            Renderer?.Dispose();
            Scenes.ClearAllScenes();
        }

        _Disposed = true;
    }

    public static Window Create(int width, int height, string title, string? iconResource, SDL_WindowFlags windowFlags, Renderer? renderer)
    {
        Window result = new(windowFlags);
        result.Initialize(width, height, title, iconResource);
        if (renderer is null)
        {
            result.CreateAndAttachRenderer();
        }
        else
        {
            result.AttachRenderer(renderer);
        }
        result.SetRenderClearColor(100, 149, 237);

        return result;
    }
}