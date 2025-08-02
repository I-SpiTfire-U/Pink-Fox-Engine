using PinkFox.Core.Scenes;

namespace PinkFox.GameTemplate.Scenes;

public class SampleScene : IScene, IDisposable
{
    private bool _Disposed;
    public event Action? OnRequestExit;

    public SampleScene(nint renderer) { }

    public void Update(float deltaTime)
    {
        // Update game objects and code below:

    }

    public void FixedUpdate()
    {
        // Update physics code and fixed values here:

    }

    public void Draw(nint renderer)
    {
        // Draw graphics to the screen below:

    }

    public void OnWindowResize(int width, int height)
    {
        // Adjust camera, UI elements, screen-space effects, etc.
        
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