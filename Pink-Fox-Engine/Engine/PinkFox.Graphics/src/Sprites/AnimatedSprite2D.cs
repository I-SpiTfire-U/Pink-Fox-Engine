using System.Numerics;
using SDL;
using PinkFox.Graphics.Rendering;
using PinkFox.Graphics.Animations;
using PinkFox.Core.Modules.Graphics;
using PinkFox.Core.Types;

namespace PinkFox.Graphics.Sprites;

public class AnimatedSprite2D : ISprite2D, IDisposable
{
    private readonly IAnimationController _AnimationController;
    public FRect SourceRect => _AnimationController.GetCurrentFrameRect();

    public string Name { get; init; }
    public Vector2 TextureSize { get; protected set; }
    public Texture2D Texture { get; protected set; }

    public Vector2 Position { get; protected set; }
    public Vector2 Scale { get; protected set; }
    public double Rotation { get; protected set; }
    public SDL_FlipMode FlipMode { get; protected set; }
    public int Layer { get; protected set; }
    public bool IsVisible { get; protected set; }
    public double RotationDegrees => Rotation * (180.0 / Math.PI);

    private bool _Disposed = false;

    public AnimatedSprite2D(string name, IAnimationController animationController, Texture2D texture, Vector2 position, Vector2? scale = null, double rotation = 0.0f, SDL_FlipMode flipMode = SDL_FlipMode.SDL_FLIP_NONE, int layer = 0, bool isVisible = true)
    {
        Name = name;
        _AnimationController = animationController;
        Texture = texture;
        TextureSize = new(Texture.Width, Texture.Height);

        Position = position;
        Scale = scale ?? TextureSize;
        Rotation = rotation;

        FlipMode = flipMode;
        Layer = layer;
        IsVisible = isVisible;
    }

    public void SetCurrentAnimation(string key, bool reset = true)
    {
        _AnimationController.SetCurrentAnimation(key, reset);
        UpdateTextureSize();
    }

    public void SetCurrentFrame(int index)
    {
        _AnimationController.SetCurrentFrame(index);
        UpdateTextureSize();
    }

    public void UpdateCurrentFrame(int amount)
    {
        _AnimationController.UpdateCurrentFrame(amount);
        UpdateTextureSize();
    }

    private void UpdateTextureSize()
    {
        SDL_FRect frameRect = _AnimationController.GetCurrentFrameRect();
        TextureSize = new Vector2(frameRect.w, frameRect.h);

        if (Scale == Vector2.Zero || Scale == TextureSize)
        {
            Scale = TextureSize;
        }
    }

    public unsafe void Draw(Renderer renderer, ICamera2D? camera2D = null)
    {
        if (!IsVisible)
        {
            return;
        }

        Vector2 topLeft = (camera2D?.WorldToScreen(Position) ?? Position) - Scale / 2f;
        float zoom = camera2D?.Zoom ?? 1f;

        FRect destinationRect = new(topLeft.X, topLeft.Y, Scale.X * zoom, Scale.Y * zoom);

        FPoint zoomedPosition = new(Position.X * zoom, Position.Y * zoom);

        Texture.Draw(renderer, destinationRect, SourceRect, RotationDegrees, zoomedPosition, FlipMode);
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