using SDL3;
using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Scenes;
using PinkFox.Core.Collisions;

namespace PinkFox.Graphics.Rendering;

public class AnimatedSprite2D : ISprite2D, IDisposable
{
    public string Name { get; init; }
    protected string CurrentAnimationKey;
    private Animation CurrentAnimation => _Animations[CurrentAnimationKey];
    private readonly Dictionary<string, Animation> _Animations;

    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public double Rotation { get; set; }
    public SDL.FlipMode FlipMode { get; set; }
    public bool IsVisible { get; set; }

    private bool _Disposed = false;

    public Vector2 TextureSize => new(CurrentAnimation.GetCurrentFrameRect().W, CurrentAnimation.GetCurrentFrameRect().H);
    public Vector2 Center => Position + Scale / 2f;
    public RectCollider Collider => new(Position, Scale, Center);

    public AnimatedSprite2D(string name, Texture2D texture, Dictionary<string, Animation> animations, Vector2 position, Vector2? origin = null, Vector2? scale = null, double rotation = 0.0f, SDL.FlipMode flipMode = SDL.FlipMode.None, bool isVisible = true)
    {
        Name = name;
        Texture = texture;
        _Animations = animations;
        CurrentAnimationKey = animations.ElementAt(0).Key;
        Position = position;
        Scale = scale ?? new(TextureSize.X, TextureSize.Y);
        Origin = origin ?? Center;
        Rotation = rotation;
        FlipMode = flipMode;
        IsVisible = isVisible;
    }

    public void SetCurrentAnimation(string key, bool reset = true)
    {
        if (!_Animations.ContainsKey(key) || key == CurrentAnimationKey)
        {
            return;
        }

        CurrentAnimationKey = key;

        if (reset)
        {
            CurrentAnimation.SetCurrentFrame(0);
        }
    }

    public void SetCurrentFrame(int index)
    {
        _Animations[CurrentAnimationKey].SetCurrentFrame(index);
    }

    public void UpdateCurrentFrame(int amount)
    {
        _Animations[CurrentAnimationKey].UpdateCurrentFrame(amount);
    }

    public void Draw(nint renderer, ICamera2D? camera2D = null)
    {
        if (!IsVisible)
        {
            return;
        }

        Vector2 screenPos = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;

        SDL.FRect destinationRect = new()
        {
            X = screenPos.X,
            Y = screenPos.Y,
            W = Scale.X * zoom,
            H = Scale.Y * zoom
        };

        SDL.FRect sourceRect = CurrentAnimation.GetCurrentFrameRect();

        SDL.FPoint center = new()
        {
            X = Scale.X * zoom / 2f,
            Y = Scale.Y * zoom / 2f
        };

        unsafe
        {
            SDL.RenderTextureRotated(renderer, Texture.TextureHandle, (nint)(&sourceRect), (nint)(&destinationRect), Rotation, (nint)(&center), FlipMode);
        }
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

    ~AnimatedSprite2D()
    {
        Dispose(false);
    }
}