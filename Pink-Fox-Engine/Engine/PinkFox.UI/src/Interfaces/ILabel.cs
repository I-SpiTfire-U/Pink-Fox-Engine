using System.Numerics;
using PinkFox.Core.Modules.Graphics;
using PinkFox.Core.Types;
using PinkFox.UI.Effects;
using PinkFox.UI.Fonts;

namespace PinkFox.UI.Interfaces;

public interface ILabel
{
    public string? Content { get; }
    public int FontSize { get; }
    public Vector2 Position { get; }
    public BitmapFont Font { get; }
    public EffectPool Effects { get; }

    public void Update(float deltaTime);
    public void Render(Renderer renderer, ICamera2D? camera2D = null);
}