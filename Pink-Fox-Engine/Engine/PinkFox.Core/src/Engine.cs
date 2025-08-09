using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Scenes;
using SDL;

namespace PinkFox.Core;

public sealed class Engine : IDisposable
{
    private int _TargetFPS = 60;
    private int _FixedUPS = 60;
    private float TargetFrameTime => 1.0f / _TargetFPS;
    private float FixedUpdateInterval => 1.0f / _FixedUPS;
    private bool _EnableFPSLimit = false;
    public void EnableFPSLimit(bool enable) => _EnableFPSLimit = enable;

    private bool _Running = true;
    private ulong _LastTicks;
    private bool _Disposed;
    private bool _FirstFrame = true;

    private unsafe SDL_Window* _Window;
    private unsafe SDL_Renderer* _Renderer;
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    public Vector2 WindowCenter => new(WindowWidth / 2, WindowHeight / 2);
    private string _WindowTitle = string.Empty;

    public unsafe SDL_Renderer* Renderer => _Renderer;

    private IInputManager? _InputManager = null;
    private IAudioManager? _AudioManager = null;
    private IVirtualRenderer? _VirtualRenderer = null;
    private SDL_WindowFlags _WindowFlags;
    private SDL_Color _ClearColor = new()
    {
        r = 100,
        g = 149,
        b = 237,
        a = 255
    };

    public IInputManager InputManager => _InputManager ?? throw new InvalidOperationException("No input manager available");
    public IAudioManager AudioManager => _AudioManager ?? throw new InvalidOperationException("No audio manager available");
    public IVirtualRenderer VirtualRenderer => _VirtualRenderer ?? throw new InvalidOperationException("No virtual renderer available");

    public void SetTargetFPS(int fps) => _TargetFPS = fps;
    public void SetFixedUPS(int ups) => _FixedUPS = ups;
    public void Stop() => _Running = false;

    public void SetInputManager(IInputManager inputManager) => _InputManager = inputManager;
    public void SetAudioManager(IAudioManager audioManager)
    {
        _AudioManager = audioManager;
        _AudioManager.Init();
    }
    public void SetVirtualRenderer(IVirtualRenderer virtualRenderer)
    {
        _VirtualRenderer = virtualRenderer;
        _VirtualRenderer.ClearColor = _ClearColor;
    }
    public void SetWindowFlags(SDL_WindowFlags windowFlags) => _WindowFlags = windowFlags;

    public unsafe void SetRenderClearColor(byte r, byte g, byte b)
    {
        _ClearColor = new()
        {
            r = r,
            g = g,
            b = b,
            a = 255
        };

        if (_VirtualRenderer is not null)
        {
            _VirtualRenderer.ClearColor = _ClearColor;
        }
        
        SDL3.SDL_SetRenderDrawColor(_Renderer, _ClearColor.r, _ClearColor.g, _ClearColor.b, _ClearColor.a);
    }

    public unsafe void SetRenderDrawColor(SDL_Color color)
    {
        SDL3.SDL_SetRenderDrawColor(_Renderer, color.r, color.g, color.b, color.a);
    }

    public unsafe void ToggleFullscreen()
    {
        SDL_WindowFlags flags = SDL3.SDL_GetWindowFlags(_Window);
        bool isFullscreen = (flags & SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) != 0;

        bool result = SDL3.SDL_SetWindowFullscreen(_Window, !isFullscreen);
        if (!result)
        {
            Console.WriteLine($"Failed to set window mode: {SDL3.SDL_GetError()}");
        }
    }

    public unsafe void InitializeWindowAndRenderer(string windowTitle, string iconPath, int windowWidth, int windowHeight)
    {
        _WindowTitle = windowTitle;
        WindowWidth = windowWidth;
        WindowHeight = windowHeight;
        _LastTicks = SDL3.SDL_GetTicks();

        SDL_InitFlags initFlags = SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_AUDIO | SDL_InitFlags.SDL_INIT_GAMEPAD | SDL_InitFlags.SDL_INIT_JOYSTICK;

        if (!SDL3.SDL_Init(initFlags))
        {
            throw new Exception($"SDL could not initialize: {SDL3.SDL_GetError()}");
        }

        _Window = SDL3.SDL_CreateWindow(_WindowTitle, WindowWidth, WindowHeight, _WindowFlags);
        if (_Window is null)
        {
            throw new Exception($"Failed to create window: {SDL3.SDL_GetError()}");
        }

        _Renderer = SDL3.SDL_CreateRenderer(_Window, (byte*)null);
        if (_Renderer is null)
        {
            throw new Exception($"Failed to create renderer: {SDL3.SDL_GetError()}");
        }

        SetWindowIcon(iconPath);

        SDL3_ttf.TTF_Init();
    }

    public unsafe void Run()
    {
        float accumulator = 0;
        while (_Running)
        {
            ulong nowTicks = SDL3.SDL_GetTicks();
            float deltaTime = (nowTicks - _LastTicks) / 1000f;

            _LastTicks = nowTicks;
            deltaTime = MathF.Min(deltaTime, 0.05f);

            accumulator = MathF.Min(accumulator + deltaTime, 0.05f);

            SDL_Event sdlEvent;
            while (SDL3.SDL_PollEvent(&sdlEvent))
            {
                _InputManager?.ProcessEvent(sdlEvent);

                SDL_EventType eventType = sdlEvent.Type;
                switch (eventType)
                {
                    case SDL_EventType.SDL_EVENT_QUIT:
                        _Running = false;
                        return;

                    case SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                        HandleWindowResize(sdlEvent.window.data1, sdlEvent.window.data2);
                        break;
                }
            }

            if (_FirstFrame)
            {
                _FirstFrame = false;
                continue;
            }

            while (accumulator >= FixedUpdateInterval)
            {
                SceneManager.FixedUpdate(FixedUpdateInterval);
                accumulator -= FixedUpdateInterval;
            }

            SceneManager.Update(deltaTime);

            if (_VirtualRenderer is not null)
            {
                _VirtualRenderer.Begin(_Renderer);
                SceneManager.Draw(_Renderer);
                _VirtualRenderer.End(_Renderer);
            }
            else
            {
                SDL3.SDL_RenderClear(_Renderer);
                SceneManager.Draw(_Renderer);
            }

            SDL3.SDL_RenderPresent(_Renderer);

            _InputManager?.Clear();

            if (_EnableFPSLimit)
            {
                float frameEnd = SDL3.SDL_GetTicks() - nowTicks;
                float delay = TargetFrameTime * 1000 - frameEnd;
                if (delay > 0f)
                {
                    SDL3.SDL_Delay((uint)delay);
                }
            }
        }

        Dispose();
    }

    public void SetWindowSize(int width, int height)
    {
        HandleWindowResize(width, height);
    }

    private unsafe void HandleWindowResize(int width, int height)
    {
        SDL3.SDL_SetWindowSize(_Window, width, height);

        WindowWidth = width;
        WindowHeight = height;

        SDL_Rect rect = new()
        {
            x = 0,
            y = 0,
            w = WindowWidth,
            h = WindowHeight
        };

        SDL3.SDL_SetRenderViewport(_Renderer, &rect);
        SceneManager.GetActiveScene()?.OnWindowResize(width, height);
    }

    private void SetWindowIcon(string iconPath)
    {
        unsafe
        {
            SDL_Surface* surface = SDL3_image.IMG_Load(iconPath);
            if (surface is null)
            {
                Console.WriteLine($"Failed to load icon: {SDL3.SDL_GetError()}");
                return;
            }

            SDL3.SDL_SetWindowIcon(_Window, surface);
            SDL3.SDL_free(surface);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }

        if (disposing)
        {
            unsafe
            {
                if (_Renderer is not null)
                {
                    SDL3.SDL_DestroyRenderer(_Renderer);
                }

                if (_Window is not null)
                {
                    SDL3.SDL_DestroyWindow(_Window);
                }
            }

            SceneManager.ClearAllScenes();
            _AudioManager?.Shutdown();
            _InputManager?.Dispose();
            SDL3_ttf.TTF_Quit();
            SDL3.SDL_Quit();
        }

        _Disposed = true;
    }
}
