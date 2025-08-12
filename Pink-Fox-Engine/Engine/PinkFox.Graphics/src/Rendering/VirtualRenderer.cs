using System.Numerics;
using PinkFox.Core.Components;
using SDL;

namespace PinkFox.Graphics.Rendering;

public class VirtualRenderer : IVirtualRenderer
{
    public SDL_Color BorderColor { get; set; } = new SDL_Color { r = 0, g = 0, b = 0, a = 255 };
    public SDL_Color ClearColor { get; set; }
    public bool UseIntegerScaling { get; set; }
    public int VirtualWidth { get; init; }
    public int VirtualHeight { get; init; }
    private readonly Texture2D _RenderTarget;

    public unsafe VirtualRenderer(SDL_Renderer* renderer, int virtualWidth, int virtualHeight, bool useIntegerScaling = false)
    {
        VirtualWidth = virtualWidth;
        VirtualHeight = virtualHeight;
        _RenderTarget = new(renderer, virtualWidth, virtualHeight);
        UseIntegerScaling = useIntegerScaling;
    }

    public unsafe void Begin(SDL_Renderer* actualRenderer)
    {
        _RenderTarget.SetAsRenderTarget(actualRenderer);
        SDL3.SDL_SetRenderDrawColor(actualRenderer, ClearColor.r, ClearColor.g, ClearColor.b, ClearColor.a);
        SDL3.SDL_RenderClear(actualRenderer);
    }

    public unsafe void End(SDL_Renderer* actualRenderer)
    {
        SDL3.SDL_SetRenderTarget(actualRenderer, null);

        int winW = 0;
        int winH = 0;
        SDL3.SDL_GetRenderOutputSize(actualRenderer, &winW, &winH);

        SDL3.SDL_SetRenderDrawColor(actualRenderer, BorderColor.r, BorderColor.g, BorderColor.b, BorderColor.a);
        SDL3.SDL_RenderClear(actualRenderer);

        float scaleX = (float)winW / VirtualWidth;
        float scaleY = (float)winH / VirtualHeight;
        float scale = Math.Min(scaleX, scaleY);

        int intScale = UseIntegerScaling ? Math.Max(1, (int)Math.Floor(scale)) : 1;

        int drawW = UseIntegerScaling ? VirtualWidth * intScale : (int)(VirtualWidth * scale);
        int drawH = UseIntegerScaling ? VirtualHeight * intScale : (int)(VirtualHeight * scale);

        int offsetX = (winW - drawW) / 2;
        int offsetY = (winH - drawH) / 2;

        SDL_FRect dest = new SDL_FRect
        {
            x = offsetX,
            y = offsetY,
            w = drawW,
            h = drawH
        };

        SDL3.SDL_SetTextureScaleMode(_RenderTarget.TextureHandle, SDL_ScaleMode.SDL_SCALEMODE_NEAREST);
        _RenderTarget.Draw(actualRenderer, dest);
    }

    public unsafe Vector2 WindowToVirtualCoords(SDL_Renderer* renderer, Vector2 windowPosition)
    {
        int winW = 0;
        int winH = 0;
        SDL3.SDL_GetRenderOutputSize(renderer, &winW, &winH);

        float scaleX = (float)winW / VirtualWidth;
        float scaleY = (float)winH / VirtualHeight;
        float scale = Math.Min(scaleX, scaleY);
        float usedScale = UseIntegerScaling ? Math.Max(1, (int)Math.Floor(scale)) : scale;

        int drawW = (int)(VirtualWidth * usedScale);
        int drawH = (int)(VirtualHeight * usedScale);

        int offsetX = (winW - drawW) / 2;
        int offsetY = (winH - drawH) / 2;

        float virtualX = (windowPosition.X - offsetX) / usedScale;
        float virtualY = (windowPosition.Y - offsetY) / usedScale;

        virtualX = Math.Clamp(virtualX, 0, VirtualWidth);
        virtualY = Math.Clamp(virtualY, 0, VirtualHeight);

        return new Vector2(virtualX, virtualY);
    }

    public unsafe SDL_Rect GetViewportRect(SDL_Renderer* renderer)
    {
        int winW = 0, winH = 0;
        SDL3.SDL_GetRenderOutputSize(renderer, &winW, &winH);

        float scaleX = (float)winW / VirtualWidth;
        float scaleY = (float)winH / VirtualHeight;
        float scale = Math.Min(scaleX, scaleY);
        float usedScale = UseIntegerScaling ? Math.Max(1, (int)Math.Floor(scale)) : scale;

        int drawW = (int)(VirtualWidth * usedScale);
        int drawH = (int)(VirtualHeight * usedScale);

        int offsetX = (winW - drawW) / 2;
        int offsetY = (winH - drawH) / 2;

        return new SDL_Rect { x = offsetX, y = offsetY, w = drawW, h = drawH };
    }
}
