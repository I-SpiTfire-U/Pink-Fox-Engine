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
    private float _Accumulator;
    private ulong _LastTicks;
    private bool _Disposed;
    private bool _FirstFrame = true;

    private unsafe SDL_Window* _Window;
    private unsafe SDL_Renderer* _Renderer;
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    private string _WindowTitle = string.Empty;

    public unsafe SDL_Renderer* Renderer => _Renderer;

    private IInputManager? _InputManager = null;
    private IAudioManager? _AudioManager = null;
    
    public IInputManager InputManager => _InputManager ?? throw new InvalidOperationException("No input manager available");
    public IAudioManager AudioManager => _AudioManager ?? throw new InvalidOperationException("No audio manager available");
    public IInputManager? OptionalInputManager => _InputManager;
    public IAudioManager? OptionalAudioManager => _AudioManager;

    public Action? OnStart { get; set; }

    public void SetTargetFPS(int fps) => _TargetFPS = fps;
    public void SetFixedUPS(int ups) => _FixedUPS = ups;
    public void Stop() => _Running = false;

    public void SetInputManager(IInputManager inputManager) => _InputManager = inputManager;
    public void SetAudioManager(IAudioManager audioManager) => _AudioManager = audioManager;
    public void SetWindowFlags(SDL_WindowFlags windowFlags) => _WindowFlags = windowFlags;
    private SDL_WindowFlags _WindowFlags;

    public void Initialize(string title, string iconPath, int width, int height)
    {
        _WindowTitle = title;
        WindowWidth = width;
        WindowHeight = height;
        _Accumulator = 0;
        _LastTicks = SDL3.SDL_GetTicks();

        SDL_InitFlags initFlags = SDL_InitFlags.SDL_INIT_VIDEO;

        if (_AudioManager is not null)
        {
            initFlags |= SDL_InitFlags.SDL_INIT_AUDIO;
        }

        if (_InputManager is not null)
        {
            initFlags |= SDL_InitFlags.SDL_INIT_GAMEPAD | SDL_InitFlags.SDL_INIT_JOYSTICK;
        }

        if (!SDL3.SDL_Init(initFlags))
        {
            throw new Exception($"SDL could not initialize: {SDL3.SDL_GetError()}");
        }

        unsafe
        {
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

            if (_Window is null || _Renderer is null)
            {
                SDL3.SDL_Quit();
                throw new Exception("Window or Renderer creation failed.");
            }

            SDL3.SDL_SetRenderDrawColor(_Renderer, 100, 149, 237, 0);
        }

        SetWindowIcon(iconPath);

        SDL3_ttf.TTF_Init();
        _AudioManager?.Init();
    }

    public void Run()
    {
        while (_Running)
        {
            ulong nowTicks = SDL3.SDL_GetTicks();
            float deltaTime = (nowTicks - _LastTicks) / 1000f;

            _LastTicks = nowTicks;
            deltaTime = MathF.Min(deltaTime, 1f);

            _Accumulator = MathF.Min(_Accumulator + deltaTime, 0.50f);

            SDL_Event sdlEvent;
            unsafe
            {
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
            }

            if (_FirstFrame)
            {
                OnStart?.Invoke();
                _FirstFrame = false;
                continue;
            }

            while (_Accumulator >= FixedUpdateInterval)
            {
                SceneManager.FixedUpdate(FixedUpdateInterval);
                _Accumulator -= FixedUpdateInterval;
            }

            SceneManager.Update(deltaTime);

            float alpha = _Accumulator / FixedUpdateInterval;
            unsafe
            {
                SDL3.SDL_RenderClear(_Renderer);
                SceneManager.Draw(_Renderer, alpha);
                SDL3.SDL_RenderPresent(_Renderer);
            }

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

    public void HandleWindowResize(int width, int height)
    {
        WindowWidth = width;
        WindowHeight = height;

        unsafe
        {
            SDL_Rect rect = new()
            {
                x = 0,
                y = 0,
                w = WindowWidth,
                h = WindowHeight
            };
            SDL3.SDL_SetRenderViewport(_Renderer, &rect);
        }
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
            
            SceneManager.UnloadScene();
            _AudioManager?.Shutdown();
            _InputManager?.Dispose();
            SDL3_ttf.TTF_Quit();
            SDL3.SDL_Quit();
        }

        _Disposed = true;
    }
}
