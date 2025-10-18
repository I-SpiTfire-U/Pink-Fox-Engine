using PinkFox.Core.Scenes;
using PinkFox.Core.Types;
using SDL;

namespace PinkFox.Core;

public sealed class Engine : IDisposable
{
    public bool FPSIsLimited;
    public int TargetFramesPerSecond { get; private set; }
    public int FixedUpdatesPerSecond { get; private set; }
    private readonly List<Window> _Windows = [];
    private readonly List<Window> _WindowsToRemove = [];

    private float TargetFrameTime;
    private float FixedUpdateInterval;

    private bool _EngineIsRunning = true;
    private bool _Disposed;

    public void Initialize()
    {
        SdlInitialization.TryInitializeSdl(SDL_InitFlags.SDL_INIT_VIDEO |
                                           SDL_InitFlags.SDL_INIT_AUDIO |
                                           SDL_InitFlags.SDL_INIT_GAMEPAD |
                                           SDL_InitFlags.SDL_INIT_JOYSTICK);
        SdlInitialization.TryInitializeMix(SDL3_mixer.MIX_INIT_MP3 |
                                           SDL3_mixer.MIX_INIT_OGG |
                                           SDL3_mixer.MIX_INIT_WAVPACK);
        SDL3_ttf.TTF_Init();
    }

    public void AddWindow(Window window)
    {
        window.OnRequestClose = _WindowsToRemove.Add;
        window.OnRequestExitProgram = RequestExit;
        _Windows.Add(window);
    }

    public void SetTargetFPS(int fps)
    {
        TargetFramesPerSecond = fps;
        TargetFrameTime = 1.0f / TargetFramesPerSecond;
    }

    public void SetFixedUPS(int ups)
    {
        FixedUpdatesPerSecond = ups;
        FixedUpdateInterval = 1.0f / FixedUpdatesPerSecond;
    }

    private void RequestExit()
    {
        _EngineIsRunning = false;
    }

    public unsafe void Run()
    {
        float accumulator = 0;
        ulong lastTicks = SDL3.SDL_GetTicks();

        while (_EngineIsRunning && _Windows.Count > 0)
        {
            ulong currentTicks = SDL3.SDL_GetTicks();
            float deltaTime = MathF.Min((currentTicks - lastTicks) / 1000f, 0.05f);
            lastTicks = currentTicks;
            accumulator = MathF.Min(accumulator + deltaTime, 0.05f);

            ProcessSdlEvents();

            foreach (Window window in _Windows)
            {
                Update(deltaTime, ref accumulator, window);
                Render(deltaTime, window);
            }

            foreach (Window window in _WindowsToRemove)
            {
                window.Dispose();
                _Windows.Remove(window);
            }
            _WindowsToRemove.Clear();

            TryLimitFps(currentTicks);
        }

        Debugging.Terminal.LogMessage(Debugging.LogLevel.Success, "Engine Exiting");
        Dispose();
    }

    private unsafe void ProcessSdlEvents()
    {
        SDL_Event sdlEvent;

        while (SDL3.SDL_PollEvent(&sdlEvent))
        {
            SDL_Event sdlEventCopy = sdlEvent;
            
            Window? window = _Windows.FirstOrDefault(w => w.WindowId == sdlEventCopy.window.windowID);
            switch (sdlEvent.Type)
            {
                case SDL_EventType.SDL_EVENT_WINDOW_CLOSE_REQUESTED:
                    window?.OnRequestClose?.Invoke(window);
                    break;

                case SDL_EventType.SDL_EVENT_QUIT:
                    _EngineIsRunning = false;
                    break;

                case SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                    window?.SetSize(sdlEventCopy.window.data1, sdlEventCopy.window.data2);
                    break;

                default:
                    IScene? activeScene = window?.Scenes.GetActiveScene();
                    activeScene?.OnSdlEventProcessed(sdlEventCopy);
                    
                    break;
            }
        }
    }

    private void Update(float deltaTime, ref float accumulator, Window window)
    {
        while (accumulator >= FixedUpdateInterval)
        {
            window.Scenes.FixedUpdate(FixedUpdateInterval);
            accumulator -= FixedUpdateInterval;
        }

        window.Scenes.Update(deltaTime);
    }

    private unsafe void Render(float deltaTime, Window window)
    {
        if (window.Renderer is null)
        {
            return;
        }
        
        if (window.Renderer.VirtualRenderer is null)
        {
            SDL3.SDL_SetRenderDrawColor(window.Renderer, window.Renderer.ClearColor.r, window.Renderer.ClearColor.g, window.Renderer.ClearColor.b, window.Renderer.ClearColor.a);
            SDL3.SDL_RenderClear(window.Renderer);
            window.Scenes.Render(deltaTime);
        }
        else
        {
            window.Renderer.BeginVirtualRenderer();
            window.Scenes.Render(deltaTime);
            window.Renderer.EndVirtualRenderer();
        }
        
        SDL3.SDL_RenderPresent(window.Renderer);
    }

    private void TryLimitFps(ulong currentTicks)
    {
        if (!FPSIsLimited)
        {
            return;
        }
        
        float frameTime = SDL3.SDL_GetTicks() - currentTicks;
        float delay = TargetFrameTime * 1000 - frameTime;

        if (delay > 0f)
        {
            SDL3.SDL_Delay((uint)delay);
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
            foreach (Window window in _Windows)
            {
                window.Dispose();
            }
            _Windows.Clear();

            SDL3_ttf.TTF_Quit();
            SDL3.SDL_Quit();
        }

        _Disposed = true;
    }
}