using PinkFox.Core.Modules.Input;
using SDL;

namespace PinkFox.UI;

public interface IButton
{
    event Action? IsHovered;
    event Action? IsClicked;

    void Update(float deltaTime, IMouse mouse);
    unsafe void Draw(SDL_Renderer* renderer);
}