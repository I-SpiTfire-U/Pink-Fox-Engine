using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Cameras;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Sprites;
using SDL;

namespace PinkFox.Sample2.Scenes;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Engine Engine;
    protected readonly Camera2D Camera;

    protected readonly Texture2D LightTexture;
    protected readonly Texture2D BlockTexture;
    protected readonly Sprite2D BlockSprite;
    protected readonly Sprite2D LightSprite;

    protected readonly SpritePool SpritePool;
    
    private bool _Disposed;

    public unsafe Scene(Engine engine)
    {
        Engine = engine;

        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(Engine.WindowWidth, Engine.WindowHeight);

        BlockTexture = TextureFactory.CreateRectangle(Engine.Renderer, 200, 200, new SDL_Color() { r = 100, g = 100, b = 100, a = 255 });
        LightTexture = TextureFactory.CreateCircleGradient(Engine.Renderer, 500, 500, new SDL_Color() { r = 255, g = 244, b = 214, a = 255 });
        SDL3.SDL_SetTextureBlendMode(LightTexture.TextureHandle, SDL_BlendMode.SDL_BLENDMODE_BLEND);

        BlockSprite = new("Block", BlockTexture, new(100, 100), null, null, null, 0d, SDL_FlipMode.SDL_FLIP_NONE, 0, true);
        LightSprite = new("Light", LightTexture, new(100, 100), null, null, null, 0d, SDL_FlipMode.SDL_FLIP_NONE, 1, true);

        SpritePool = new();
        SpritePool.AddRange(BlockSprite, LightSprite);
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:
        
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:

        if (Engine.InputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_A))
        {
            BlockSprite.Position = new(BlockSprite.Position.X - 100f * deltaTime, BlockSprite.Position.Y);
        }
        if (Engine.InputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_D))
        {
            BlockSprite.Position = new(BlockSprite.Position.X + 100f * deltaTime, BlockSprite.Position.Y);
        }
        if (Engine.InputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_W))
        {
            BlockSprite.Position = new(BlockSprite.Position.X, BlockSprite.Position.Y - 100f * deltaTime);
        }
        if (Engine.InputManager.Keyboard.IsKeyHeld(SDL_Keycode.SDLK_S))
        {
            BlockSprite.Position = new(BlockSprite.Position.X, BlockSprite.Position.Y + 100f * deltaTime);
        }

        if (Engine.InputManager.Keyboard.IsKeyDown(SDL_Keycode.SDLK_ESCAPE))
        {
            Engine.Stop();
        }
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:

    }

    public unsafe void Draw(SDL_Renderer* renderer)
    {
        // TODO: Draw game elements to the screen below:

        foreach (ISprite2D sprite in SpritePool.GetSortedByLayer())
        {
            if (sprite.IsVisible && Camera.SpriteIsInView(sprite))
            {
                sprite.Draw(renderer, Camera);
                Console.WriteLine(sprite.Name);
            }
        }
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