using PinkFox.Core.Collisions;
using PinkFox.Core.Modules.Input;
using PinkFox.Core.Types;
using PinkFox.Graphics;
using PinkFox.Graphics.Rendering;
using PinkFox.UI.Interfaces;

namespace PinkFox.UI.Types;

public class Button : IButton
{
    public RectCollider2D Bounds { get; }
    public Texture2D Texture { get; }
    
    public event Action? IsHovered;
    public event Action? IsClicked;

    public Button(RectCollider2D bounds, Texture2D texture)
    {
        Bounds = bounds;
        Texture = texture;
    }

    public Button(RectCollider2D bounds, Renderer renderer)
    {
        Bounds = bounds;
        Texture = TextureFactory.CreateRectangleOutline(renderer, (int)bounds.Scale.X, (int)bounds.Scale.Y, 2, Color.White);
    }

    public void Update(float deltaTime, IMouse mouse)
    {
        RectCollider2D mouseBounds = new(mouse.Position, 1, 1);

        if (mouseBounds.IsCollidingWith(Bounds))
        {
            IsHovered?.Invoke();

            if (mouse.IsButtonDown(SDL.SDL_MouseButtonFlags.SDL_BUTTON_LMASK))
            {
                IsClicked?.Invoke();
            }
        }
    }
    
    public void Render(Renderer renderer)
    {
        Texture.Render(
            renderer,
            new FRect(0, 0, Texture.Width, Texture.Height),
            new FRect(Bounds.Position.X, Bounds.Position.Y, Texture.Width, Texture.Height)
        );
    }
}