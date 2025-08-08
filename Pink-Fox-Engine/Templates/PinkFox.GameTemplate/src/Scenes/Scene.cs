using PinkFox.Core;
using PinkFox.Core.Scenes;
using SDL;

namespace PinkFox.GameTemplate.Scenes;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Engine Engine;
    
    private bool _Disposed;

    public unsafe Scene(Engine engine)
    {
        Engine = engine;
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:
        
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:

    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:

    }

    public unsafe void Draw(SDL_Renderer* renderer)
    {
        // TODO: Draw game elements to the screen below:

    }

    public void OnWindowResize(int windowWidth, int windowHeight)
    {
        // TODO: Handle logic that should respond to window size changes below:
        
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
        
        if (disposing) { }

        _Disposed = true;
    }
}