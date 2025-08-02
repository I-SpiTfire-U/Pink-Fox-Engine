using PinkFox.Core.Interfaces;
using PinkFox.Core.Scenes;
using SDL3;

namespace PinkFox.Core;

public sealed class Engine : IDisposable
{
    private int _TargetFPS = 60;
    private int _FixedUPS = 60;

    private float _TargetFrameTime => 1.0f / _TargetFPS;
    private float _FixedUpdateInterval => 1.0f / _FixedUPS;

    private bool _Running = true;
    private float _Accumulator;
    private ulong _LastTicks;
    private bool _Disposed;
    private bool _FirstFrame = true;

    private nint _Window;
    private nint _Renderer;
    private string _WindowTitle = string.Empty;
    private int _WindowWidth;
    private int _WindowHeight;

    public nint Renderer => _Renderer;

    public IInputManager? InputManager { get; set; }
    public IAudioManager? AudioManager { get; set; }
    public Action? OnStart { get; set; }

    public void SetTargetFPS(int fps) => _TargetFPS = fps;
    public void SetFixedUPS(int ups) => _FixedUPS = ups;
    public void Stop() => _Running = false;

    public void Initialize(string title, int width, int height)
    {
        _WindowTitle = title;
        _WindowWidth = width;
        _WindowHeight = height;
        _Accumulator = 0;
        _LastTicks = SDL.GetTicks();

        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Audio | SDL.InitFlags.Gamepad | SDL.InitFlags.Joystick))
        {
            throw new Exception($"SDL could not initialize: {SDL.GetError()}");
        }

        if (!SDL.CreateWindowAndRenderer(_WindowTitle, _WindowWidth, _WindowHeight, SDL.WindowFlags.Resizable, out _Window, out _Renderer))
        {
            SDL.Quit();
            throw new Exception($"Error creating window/renderer: {SDL.GetError()}");
        }

        if (_Window == 0 || _Renderer == 0)
        {
            SDL.Quit();
            throw new Exception("Window or Renderer creation failed.");
        }

        TTF.Init();

        SDL.SetRenderDrawColor(_Renderer, 100, 149, 237, 0);
        AudioManager?.Init();
    }

    public void Run()
    {
        while (_Running)
        {
            ulong nowTicks = SDL.GetTicks();
            float deltaTime = (nowTicks - _LastTicks) / 1000f;
            _LastTicks = nowTicks;
            _Accumulator += deltaTime;

            while (SDL.PollEvent(out SDL.Event sdlEvent))
            {
                InputManager?.ProcessEvent(sdlEvent);

                switch ((SDL.EventType)sdlEvent.Type)
                {
                    case SDL.EventType.Quit:
                        _Running = false;
                        return;

                    case SDL.EventType.WindowResized:
                        int newWidth = sdlEvent.Window.Data1;
                        int newHeight = sdlEvent.Window.Data2;
                        HandleWindowResize(newWidth, newHeight);
                        break;
                }
            }


            if (_FirstFrame)
            {
                OnStart?.Invoke();
                _FirstFrame = false;
                continue;
            }

            SceneManager.Update(deltaTime);

            while (_Accumulator >= _FixedUpdateInterval)
            {
                SceneManager.FixedUpdate();
                _Accumulator -= _FixedUpdateInterval;
            }

            SDL.RenderClear(_Renderer);
            SceneManager.Draw(_Renderer);
            SDL.RenderPresent(_Renderer);

            InputManager?.Clear();

            uint frameTime = (uint)(SDL.GetTicks() - nowTicks);
            int delay = (int)(_TargetFrameTime * 1000) - (int)frameTime;
            if (delay > 0)
            {
                SDL.Delay((uint)delay);
            }
        }
    }

    public void HandleWindowResize(int width, int height)
    {
        _WindowWidth = width;
        _WindowHeight = height;

        SDL.RenderViewportSet(Renderer);
        SceneManager.ActiveScene?.OnWindowResize(width, height);
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
            AudioManager?.Shutdown();
            SDL.Quit();
        }

        _Disposed = true;
    }
}
