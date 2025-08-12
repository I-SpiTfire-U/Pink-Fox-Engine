using System.Numerics;
using PinkFox.Core.Components;
using PinkFox.Core.Collisions;
using SDL;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Animations;

namespace PinkFox.Graphics.Sprites;

public class AnimatedSprite2D : ISprite2D, IDisposable
{
    private readonly IAnimationController _AnimationController;
    public SDL_FRect SourceRect => _AnimationController.GetCurrentFrameRect();

    public string Name { get; init; }
    public Vector2 TextureSize { get; protected set; }
    public Texture2D Texture { get; protected set; }

    public Vector2 Position { get; set; }
    public Vector2 Origin { get; set; }
    public Vector2 Scale { get; set; }
    public double Rotation { get; set; }
    public SDL_FlipMode FlipMode { get; set; }
    public int Layer { get; set; }
    public bool IsVisible { get; set; }

    private bool _Disposed = false;

    public Vector2 Center => Position + Scale / 2f;
    public ICollider Collider => new BoxCollider(Position, Scale);
    public double RotationDegrees => Rotation * (180.0 / Math.PI);

    public AnimatedSprite2D(string name, IAnimationController animationController, Texture2D texture, Vector2 position, Vector2? origin = null, Vector2? scale = null, double rotation = 0.0f, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE, int layer = 0, bool isVisible = true)
    {
        Name = name;
        _AnimationController = animationController;
        Texture = texture;
        TextureSize = new(Texture.Width, Texture.Height);

        Position = position;
        Scale = scale ?? TextureSize;
        Origin = origin ?? Center;
        Rotation = rotation;
        FlipMode = flipMode;
        Layer = layer;
        IsVisible = isVisible;
    }

    public void SetCurrentAnimation(string key, bool reset = true)
    {
        _AnimationController.SetCurrentAnimation(key, reset);
        UpdateTextureSizeAndOrigin();
    }

    public void SetCurrentFrame(int index)
    {
        _AnimationController.SetCurrentFrame(index);
        UpdateTextureSizeAndOrigin();
    }

    public void UpdateCurrentFrame(int amount)
    {
        _AnimationController.UpdateCurrentFrame(amount);
        UpdateTextureSizeAndOrigin();
    }

    private void UpdateTextureSizeAndOrigin()
    {
        SDL_FRect frameRect = _AnimationController.GetCurrentFrameRect();
        TextureSize = new Vector2(frameRect.w, frameRect.h);

        if (Origin == Center || Origin == Vector2.Zero)
        {
            Origin = TextureSize / 2f;
        }

        if (Scale == Vector2.Zero || Scale == TextureSize)
        {
            Scale = TextureSize;
        }
    }

    public unsafe void Draw(SDL_Renderer* renderer, ICamera2D? camera2D = null)
    {
        if (!IsVisible)
        {
            return;
        }

        Vector2 screenPosition = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;

        SDL_FRect destinationRect = new()
        {
            x = screenPosition.X,
            y = screenPosition.Y,
            w = Scale.X * zoom,
            h = Scale.Y * zoom
        };

        SDL_FRect sourceRect = SourceRect;

        SDL_FPoint center = new()
        {
            x = Origin.X * zoom,
            y = Origin.Y * zoom
        };

        unsafe
        {
            SDL3.SDL_RenderTextureRotated(renderer, Texture.TextureHandle, &sourceRect, &destinationRect, RotationDegrees, &center, FlipMode);
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

    public override string ToString() => $"{Name} Pos:{Position} Scale:{Scale} Rot:{RotationDegrees}° Visible:{IsVisible}";
}