using PinkFox.Core.Scenes;
using PinkFox.Core.Types;
using SDL;

namespace PinkFox.GameTemplate.Scenes;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Window Window;
    protected Renderer Renderer => Window.Renderer!;
    
    private bool _Disposed;

    public Scene(Window window)
    {
        Window = window;
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

    public void Render(float deltaTime)
    {
        // TODO: Draw game elements to the screen below:

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