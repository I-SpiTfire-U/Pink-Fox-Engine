using PinkFox.Core.Collisions;
using PinkFox.Core.Components;
using PinkFox.Graphics.Rendering;
using SDL;

namespace PinkFox.SampleGame;

public class SimpleButton
{
    public event Action? OnClick;
    private Texture2D _Texture;
    private readonly Texture2D _DefaultTexture;
    private readonly Texture2D _HoverTexture;
    private readonly RectCollider _Collider;

    public unsafe SimpleButton(SDL_Renderer* renderer, RectCollider collider)
    {
        _Collider = collider;
        _DefaultTexture = Texture2D.CreateSquareOutlineTexture(renderer, (int)_Collider.Size.X, (int)_Collider.Size.Y, 3, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
        _HoverTexture = Texture2D.CreateSquareOutlineTexture(renderer, (int)_Collider.Size.X, (int)_Collider.Size.Y, 3, new SDL_Color() { r = 255, g = 0, b = 255, a = 255 });

        _Texture = _DefaultTexture;
    }

    public void Update(IMouse mouse)
    {
        RectCollider mouseBounds = new(mouse.Position, 1, 1);
        if (_Collider.IsCollidingWith(mouseBounds))
        {
            _Texture = _HoverTexture;
            if (mouse.IsButtonDown(SDL_MouseButtonFlags.SDL_BUTTON_LMASK))
            {
                OnClick?.Invoke();
            }
        }
        else
        {
            _Texture = _DefaultTexture;
        }
    }

    public unsafe void Draw(SDL_Renderer* renderer)
    {
        _Texture.Draw(renderer, _Collider.Position);
    }
}