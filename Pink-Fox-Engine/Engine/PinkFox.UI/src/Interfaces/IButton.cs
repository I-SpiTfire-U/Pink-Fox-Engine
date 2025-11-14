using PinkFox.Core.Collisions;
using PinkFox.Core.Modules.Input;
using PinkFox.Core.Types;

namespace PinkFox.UI.Interfaces;

public interface IButton
{
    public RectCollider2D Bounds { get; }
    public event Action? IsHovered;
    public event Action? IsClicked;

    public void Update(float deltaTime, IMouse mouse);
    public void Render(Renderer renderer);
}