using System.Numerics;
using PinkFox.Core.Types;
using SDL;

namespace PinkFox.Core.Modules.Graphics;

public interface IVirtualRenderer
{
    SDL_Color BorderColor { get; set; }
    SDL_Color ClearColor { get; set; }
    bool UseIntegerScaling { get; set; }
    int VirtualWidth { get; init; }
    int VirtualHeight { get; init; }
    unsafe void Begin(Renderer actualRenderer);
    unsafe void End(Renderer actualRenderer);
    unsafe Vector2 WindowToVirtualCoords(Renderer renderer, Vector2 windowPosition);
    unsafe SDL_Rect GetViewportRect(Renderer renderer);
}