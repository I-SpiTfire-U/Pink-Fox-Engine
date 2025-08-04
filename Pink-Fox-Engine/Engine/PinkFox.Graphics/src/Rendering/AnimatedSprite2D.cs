using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Scenes;
using PinkFox.Core.Collisions;
using SDL;

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
    public SDL_FlipMode FlipMode { get; set; }
    public bool IsVisible { get; set; }

    private bool _Disposed = false;

    public Vector2 TextureSize => new(CurrentAnimation.GetCurrentFrameRect().w, CurrentAnimation.GetCurrentFrameRect().h);
    public Vector2 Center => Position + Scale / 2f;
    public RectCollider Collider => new(Position, Scale, Center);

    public AnimatedSprite2D(string name, Texture2D texture, Dictionary<string, Animation> animations, Vector2 position, Vector2? origin = null, Vector2? scale = null, double rotation = 0.0f, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE, bool isVisible = true)
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

    public unsafe void Draw(SDL_Renderer* renderer, ICamera2D? camera2D = null)
    {
        if (!IsVisible)
        {
            return;
        }

        Vector2 screenPos = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;

        SDL_FRect destinationRect = new()
        {
            x = screenPos.X,
            y = screenPos.Y,
            w = Scale.X * zoom,
            h = Scale.Y * zoom
        };

        SDL_FRect sourceRect = CurrentAnimation.GetCurrentFrameRect();

        SDL_FPoint center = new()
        {
            x = Scale.X * zoom / 2f,
            y = Scale.Y * zoom / 2f
        };

        unsafe
        {
            SDL3.SDL_RenderTextureRotated(renderer, Texture.TextureHandle, &sourceRect, &destinationRect, Rotation, &center, FlipMode);
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
}