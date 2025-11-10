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
    protected bool IsDrawing = false;

    public BoxSelect(Texture2D texture, int thickness)
    {
        Texture = texture;
        Thickness = thickness;
    }
    
    public void Update(Renderer renderer, IMouse mouse)
    {
        bool mouseDown = mouse.IsButtonHeld(SDL_MouseButtonFlags.SDL_BUTTON_LMASK);

        if (mouseDown && !IsDrawing)
        {
            StartPosition = mouse.Position;
            IsDrawing = true;
        }
        else if (!mouseDown)
        {
            IsDrawing = false;
        }

        if (IsDrawing)
        {
           UpdateSelection(renderer, mouse.Position); 
        }
    }

    public void Render(Renderer renderer)
    {
        if (IsDrawing)
        {
            Texture.Draw(renderer);
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

        // Draw four edges as filled rectangles
        DrawRect(renderer, x, y, w, Thickness); // top
        DrawRect(renderer, x, y + h - Thickness, w, Thickness); // bottom
        DrawRect(renderer, x, y, Thickness, h); // left
        DrawRect(renderer, x + w - Thickness, y, Thickness, h); // right

        SDL3.SDL_SetRenderTarget(renderer, null);
    }

    private static unsafe void DrawRect(Renderer renderer, int x, int y, int w, int h)
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