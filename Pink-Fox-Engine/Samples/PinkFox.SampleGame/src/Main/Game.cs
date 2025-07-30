using SDL3;
using PinkFox.SampleGame.Scenes;
using PinkFox.Core.Scenes;
using PinkFox.Input;

namespace PinkFox.SampleGame.Main;

public sealed class Game : IDisposable
{
    private const int TargetFPS = 60;
    private const int FixedUPS = 60;
    private const float TargetFrameTime = 1.0f / TargetFPS;
    private const float FixedUpdateInterval = 1.0f / FixedUPS;

    private float _Accumulator;
    private ulong _LastTicks;

    nint _Window;
    nint _Renderer;

    public void Initialize()
    {
        _Accumulator = 0.0f;
        _LastTicks = SDL.GetTicks();
        (_Window, _Renderer) = InitializeWindow();

        SampleScene sampleScene = new(_Renderer);
        SceneManager.LoadScene(sampleScene, _Renderer);
    }

    public void Run()
    {
        bool programIsRunning = true;
        while (programIsRunning)
        {
            ulong nowTicks = SDL.GetTicks();
            float deltaTime = (nowTicks - _LastTicks) / 1000.0f; // seconds
            _LastTicks = nowTicks;
            _Accumulator += deltaTime;

            while (SDL.PollEvent(out var e))
            {
                InputManager.ProcessEvent(e);

                if ((SDL.EventType)e.Type == SDL.EventType.Quit)
                {
                    programIsRunning = false;
                }
            }

            SceneManager.Update(deltaTime);
            while (_Accumulator >= FixedUpdateInterval)
            {
                SceneManager.FixedUpdate();
                _Accumulator -= FixedUpdateInterval;
            }

            InputManager.Clear();

            SDL.RenderClear(_Renderer);
            SceneManager.Draw(_Renderer);
            SDL.RenderPresent(_Renderer);

            uint frameTime = (uint)(SDL.GetTicks() - nowTicks);
            int delay = (int)(TargetFrameTime * 1000) - (int)frameTime;
            if (delay > 0)
            {
                SDL.Delay((uint)delay);
            }
        }

        SDL.DestroyRenderer(_Renderer);
        SDL.DestroyWindow(_Window);
        SDL.Quit();
    }

    private (nint window, nint renderer) InitializeWindow()
    {
        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Gamepad | SDL.InitFlags.Joystick))
        {
            throw new Exception($"SDL could not initialize: {SDL.GetError()}");
        }

        if (!SDL.CreateWindowAndRenderer("Basic SDL Game", 800, 600, SDL.WindowFlags.Resizable, out nint window, out nint renderer))
        {
            SDL.Quit();
            throw new Exception($"Error creating window and rendering: {SDL.GetError()}");
        }

        SDL.SetRenderDrawColor(renderer, 100, 149, 237, 0);

        return (window, renderer);
    }

    public void Dispose()
    {

    }
}