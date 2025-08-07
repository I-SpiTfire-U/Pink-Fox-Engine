using PinkFox.Core;
using PinkFox.Core.Collisions;
using PinkFox.Core.Scenes;
using PinkFox.UI.Text;
using SDL;

namespace PinkFox.SampleGame.Scenes;

public class MenuScene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Engine Engine;

    protected readonly SimpleButton Button;
    protected readonly Font ArialFont;
    
    private bool _Disposed;

    public unsafe MenuScene(Engine engine)
    {
        Engine = engine;

        ArialFont = new(@"Assets\Fonts\arial.ttf", 32f);

        RectCollider buttonCollider = new(Engine.WindowWidth / 2 - 160, Engine.WindowHeight / 2, 323, 40);
        Button = new(Engine.Renderer, buttonCollider);
        Button.OnClick += ()=> SceneManager.PushScene("Game Scene");
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:
        
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:

        Button.Update(Engine.InputManager.Mouse);

        if (Engine.InputManager.Keyboard.IsKeyDown(SDL_Keycode.SDLK_ESCAPE) || Engine.InputManager.Gamepads.IsButtonHeld(0, SDL_GamepadButton.SDL_GAMEPAD_BUTTON_START))
        {
            Engine.Stop();
        }
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:

    }

    public unsafe void Draw(SDL_Renderer* renderer, float alpha)
    {
        // TODO: Draw game elements to the screen below:

        ArialFont.DrawText(renderer, "Press Space to Start!", Engine.WindowWidth / 2 - 150, Engine.WindowHeight / 2);
        Button.Draw(renderer);
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