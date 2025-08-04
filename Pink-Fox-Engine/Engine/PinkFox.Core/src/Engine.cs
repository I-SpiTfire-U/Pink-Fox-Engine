using PinkFox.Core.Components;
using PinkFox.Core.Scenes;
using SDL3;

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

    private nint _Window;
    private nint _Renderer;
    public int WindowWidth { get; private set; }
    public int WindowHeight { get; private set; }
    private string _WindowTitle = string.Empty;

    public nint Renderer => _Renderer;

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

    public void Initialize(string title, int width, int height)
    {
        _WindowTitle = title;
        WindowWidth = width;
        WindowHeight = height;
        _Accumulator = 0;
        _LastTicks = SDL.GetTicks();

        SDL.InitFlags initFlags = SDL.InitFlags.Video;

        if (_AudioManager is not null)
        {
            initFlags |= SDL.InitFlags.Audio;
        }

        if (_InputManager is not null)
        {
            initFlags |= SDL.InitFlags.Gamepad | SDL.InitFlags.Joystick;
        }

        if (!SDL.Init(initFlags))
        {
            throw new Exception($"SDL could not initialize: {SDL.GetError()}");
        }

        if (!SDL.CreateWindowAndRenderer(_WindowTitle, WindowWidth, WindowHeight, SDL.WindowFlags.Resizable, out _Window, out _Renderer))
        {
            SDL.Quit();
            throw new Exception($"Error creating window/renderer: {SDL.GetError()}");
        }

        if (_Window == 0 || _Renderer == 0)
        {
            SDL.Quit();
            throw new Exception("Window or Renderer creation failed.");
        }

        SDL.SetRenderDrawColor(_Renderer, 100, 149, 237, 0);

        TTF.Init();
        _AudioManager?.Init();
    }

    public void Run()
    {
        while (_Running)
        {
            ulong nowTicks = SDL.GetTicks();
            float deltaTime = (nowTicks - _LastTicks) / 1000f;

            _LastTicks = nowTicks;
            deltaTime = MathF.Min(deltaTime, 1f);

            _Accumulator = MathF.Min(_Accumulator + deltaTime, 0.50f);

            while (SDL.PollEvent(out SDL.Event sdlEvent))
            {
                _InputManager?.ProcessEvent(sdlEvent);

                SDL.EventType eventType = (SDL.EventType)sdlEvent.Type;
                switch (eventType)
                {
                    case SDL.EventType.Quit:
                        _Running = false;
                        return;

                    case SDL.EventType.WindowResized:
                        HandleWindowResize(sdlEvent.Window.Data1, sdlEvent.Window.Data2);
                        break;
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

            SDL.RenderClear(_Renderer);
            float alpha = _Accumulator / FixedUpdateInterval;
            SceneManager.Draw(_Renderer, alpha);
            SDL.RenderPresent(_Renderer);

            _InputManager?.Clear();

            if (_EnableFPSLimit)
            {
                float frameEnd = SDL.GetTicks() - nowTicks;
                float delay = TargetFrameTime * 1000 - frameEnd;
                if (delay > 0f)
                {
                    SDL.Delay((uint)delay);
                }
            }
        }
    }

    public void HandleWindowResize(int width, int height)
    {
        WindowWidth = width;
        WindowHeight = height;

        SDL.RenderViewportSet(Renderer);
        SceneManager.GetActiveScene()?.OnWindowResize(width, height);
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
            SceneManager.UnloadScene();
            if (_Renderer != 0)
            {
                SDL.DestroyRenderer(_Renderer);
            }
            if (_Window != 0)
            {
                SDL.DestroyWindow(_Window);
            }
            _AudioManager?.Shutdown();
            SDL.Quit();
        }

        _Disposed = true;
    }
}
