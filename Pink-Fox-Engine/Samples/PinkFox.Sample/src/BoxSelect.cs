using System.Numerics;
using PinkFox.Core.Modules.Input;
using PinkFox.Core.Types;
using PinkFox.Graphics.Rendering;
using SDL;

namespace PinkFox.Sample;

public class BoxSelect
{
    protected int Thickness;
    protected Texture2D Texture;
    protected Vector2 StartPosition = Vector2.Zero;
    protected bool IsRendering = false;

    public BoxSelect(Texture2D texture, int thickness)
    {
        Texture = texture;
        Thickness = thickness;
    }
    
    public void Update(Renderer renderer, IMouse mouse)
    {
        bool mouseDown = mouse.IsButtonHeld(SDL_MouseButtonFlags.SDL_BUTTON_LMASK);

        if (mouseDown && !IsRendering)
        {
            StartPosition = mouse.Position;
            IsRendering = true;
        }
        else if (!mouseDown)
        {
            IsRendering = false;
        }

        if (IsRendering)
        {
           UpdateSelection(renderer, mouse.Position); 
        }
    }

    public void Render(Renderer renderer)
    {
        if (IsRendering)
        {
            Texture.Render(renderer);
        }
    }

    private unsafe void UpdateSelection(Renderer renderer, Vector2 endPoint)
    {
        Texture.SetAsRenderTarget(renderer);

        SDL3.SDL_SetRenderDrawColor(renderer, 0, 0, 0, 0);
        SDL3.SDL_RenderClear(renderer);

        SDL3.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 255);

        int x = (int)Math.Min(StartPosition.X, endPoint.X);
        int y = (int)Math.Min(StartPosition.Y, endPoint.Y);
        int w = (int)Math.Abs(endPoint.X - StartPosition.X);
        int h = (int)Math.Abs(endPoint.Y - StartPosition.Y);

        RenderRect(renderer, x, y, w, Thickness);
        RenderRect(renderer, x, y + h - Thickness, w, Thickness);
        RenderRect(renderer, x, y, Thickness, h);
        RenderRect(renderer, x + w - Thickness, y, Thickness, h);

        SDL3.SDL_SetRenderTarget(renderer, null);
    }

    private static unsafe void RenderRect(Renderer renderer, int x, int y, int w, int h)
    {
        SDL_FRect rect = new()
        {
            x = x,
            y = y,
            w = w,
            h = h
        };
        SDL3.SDL_RenderFillRect(renderer, &rect);
    }
}