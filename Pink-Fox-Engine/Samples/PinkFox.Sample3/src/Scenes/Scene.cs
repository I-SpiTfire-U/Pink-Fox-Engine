using System.Numerics;
using PinkFox.Core;
using PinkFox.Core.Scenes;
using PinkFox.Graphics.Cameras;
using PinkFox.Graphics.Fonts;
using PinkFox.Graphics.Rendering;
using SDL;

namespace PinkFox.Sample3.Scenes;

public class Scene : IScene, IDisposable
{
    public bool HasBeenLoaded { get; set; }
    protected readonly Engine Engine;
    protected readonly Camera2D Camera;

    protected BitmapFont TestBitmapFont;
    protected Texture2D FontTexture;
    protected Label DialogueLabel;
    protected Label FPSLabel;

    protected float WaitTime = 0f;
    
    private bool _Disposed;


    public unsafe Scene(Engine engine)
    {
        Engine = engine;

        Camera = new(1f, 0.1f, 5f);
        Camera.SetViewSize(Engine.WindowWidth, Engine.WindowHeight);

        FontTexture = new("Arial_Regular_32", Engine.Renderer);
        BitmapFontData testFontData = BitmapFont.LoadFontDataFromJson("Json_Arial_Regular_32");
        TestBitmapFont = new(FontTexture, testFontData);

        DialogueLabel = new(TestBitmapFont, null, 16, new(0, 0));
        FPSLabel = new(TestBitmapFont, null, 32, new(0, 100));
        
        DialogueLabel.OnCharacterTyped += () => Engine.AudioManager.PlaySound("TypeSound");
        DialogueLabel.StartTypingEffect("HELLO, FRIEND...\nHOW ARE YOU?..", 0.4f);
        DialogueLabel.Shake(0f);
    }

    public void LoadContent()
    {
        // TODO: Load game assets such as textures, audio, and other content here:

        Engine.AudioManager.LoadSound("TypeSound", "Audio_TypeSound");
    }

    public void Update(float deltaTime)
    {
        // TODO: Update game logic that runs every frame, such as input handling, animations, or timers below:
        UpdateFps(deltaTime);
 
        DialogueLabel.Update(deltaTime);

        if (!DialogueLabel.IsTyping)
        {
            WaitTime += deltaTime;
            if (WaitTime > 5f)
            {
                WaitTime = 0f;
                DialogueLabel.StartTypingEffect("I HOPE YOU ARE WELL...", 0.4f);
            }
        }
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        // TODO: Update physics or other fixed-timestep systems below:

    }

    public unsafe void Draw(SDL_Renderer* renderer)
    {
        // TODO: Draw game elements to the screen below:
        Vector2 position = Camera.ScreenToWorld(new Vector2(20, 20));
        DialogueLabel.Position = position;
        
        DialogueLabel.Draw(renderer);
        FPSLabel.Draw(renderer);
    }

    public void OnWindowResize(int windowWidth, int windowHeight)
    {
        // TODO: Handle logic that should respond to window size changes below:
        
    }

    private int _FrameCount = 0;
    private float _ElapsedTime = 0f;
    private float _Fps = 0;

    public void UpdateFps(float deltaTime)
    {
        _FrameCount++;
        _ElapsedTime += deltaTime;

        if (_ElapsedTime >= 1f)
        {
            _Fps = _FrameCount;
            _FrameCount = 0;
            _ElapsedTime = 0f;
        }

        FPSLabel.SetText($"FPS: {_Fps:F2}");
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