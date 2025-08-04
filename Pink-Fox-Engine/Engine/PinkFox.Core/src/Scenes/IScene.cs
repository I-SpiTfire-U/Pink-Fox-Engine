using SDL;

namespace PinkFox.Core.Scenes;

public interface IScene
{
    /// <summary>
    /// Raised when the scene requests the engine to exit (e.g., game over, user quits).
    /// </summary>
    event Action? OnRequestExit;

    /// <summary>
    /// Load game assets such as textures, audio, and other content here.
    /// Called once when the scene is initialized.
    /// </summary>
    void LoadContent();

    /// <summary>
    /// Update game logic that runs every frame, such as input handling, animations, or timers.
    /// </summary>
    /// <param name="deltaTime">The time elapsed since the last frame, in seconds.</param>
    void Update(float deltaTime);

    /// <summary>
    /// Update physics or other fixed-timestep systems here.
    /// Called at consistent intervals regardless of framerate.
    /// </summary>
    /// <param name="fixedUpdateInterval">The fixed update interval, in seconds.</param>
    void FixedUpdate(float fixedUpdateInterval);

    /// <summary>
    /// Draw game elements to the screen here.
    /// </summary>
    /// <param name="renderer">The SDL renderer used for drawing.</param>
    /// <param name="alpha">Interpolation factor for smooth rendering between fixed updates (useful for physics).</param>
    unsafe void Draw(SDL_Renderer* renderer, float alpha);

    /// <summary>
    /// Handle logic that should respond to window size changes (e.g. updating camera or UI layout).
    /// </summary>
    /// <param name="windowWidth">The new window width in pixels.</param>
    /// <param name="windowHeight">The new window height in pixels.</param>
    void OnWindowResize(int windowWidth, int windowHeight);
    
    /// <summary>
    /// Clean up any resources used by the scene.
    /// </summary>
    void Dispose();
}