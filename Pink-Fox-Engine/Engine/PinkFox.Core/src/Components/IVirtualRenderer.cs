using System.Numerics;
using SDL;

namespace PinkFox.Core.Components;

public interface IVirtualRenderer
{
    SDL_Color BorderColor { get; set; }
    SDL_Color ClearColor { get; set; }
    bool UseIntegerScaling { get; set; }
    int VirtualWidth { get; init; }
    int VirtualHeight { get; init; }
    unsafe void Begin(SDL_Renderer* actualRenderer);
    unsafe void End(SDL_Renderer* actualRenderer);
    unsafe Vector2 WindowToVirtualCoords(SDL_Renderer* renderer, Vector2 windowPosition);
    unsafe SDL_Rect GetViewportRect(SDL_Renderer* renderer);
}