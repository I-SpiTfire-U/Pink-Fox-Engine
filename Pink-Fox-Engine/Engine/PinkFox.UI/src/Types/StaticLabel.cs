using System.Numerics;
using PinkFox.Core.Modules.Graphics;
using PinkFox.Core.Types;
using PinkFox.UI.Effects;
using PinkFox.UI.Fonts;
using PinkFox.UI.Interfaces;

namespace PinkFox.UI.Types;

public class StaticLabel : ILabel
{
    public Vector2 Position { get; set; }
    public BitmapFont Font { get; }
    public EffectPool Effects { get; } = new();
    public string? Content { get; private set; }
    public int FontSize { get; private set; }

    public StaticLabel(BitmapFont font, string? content = null, int fontSize = 32, Vector2? position = null)
    {
        Font = font;
        FontSize = fontSize / Font.FontData.Config.CharHeight;
        Content = content;

        if (position is not null)
        {
            Position = position.Value;
        }
    }

    public void Update(float deltaTime)
    {
        Effects.Update(deltaTime);
    }

    public unsafe void Render(Renderer renderer, ICamera2D? camera2D = null)
    {
        if (string.IsNullOrEmpty(Content))
        {
            return;
        }

        Vector2 screenPosition = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;
        float scaleWithZoom = FontSize * zoom;

        float cursorX = screenPosition.X;
        float cursorY = screenPosition.Y;
        float lineHeight = Font.LineHeight * scaleWithZoom;

        for (int i = 0; i < Content.Length; i++)
        {
            char c = Content[i];

            if (c == '\n')
            {
                cursorX = screenPosition.X;
                cursorY += lineHeight;
                continue;
            }

            if (!Font.Characters.TryGetValue(c, out GlyphInfo? glyph))
            {
                if (c == ' ')
                {
                    float spaceAdvance = Font.SpaceAdvance * scaleWithZoom;
                    cursorX += spaceAdvance;
                }
                continue;
            }

            float RenderX = cursorX + glyph.XOffset * scaleWithZoom;
            float RenderY = cursorY + (Font.FontData.Config.LineSpacing - glyph.YOffset) * scaleWithZoom;

            Vector2 basePosition = new(RenderX, RenderY);

            foreach (IEffect? effect in Effects.EffectDictionary.Values)
            {
                if (effect.IsRunning)
                {
                    basePosition += effect.ApplyEffectToCharacter(c, i, basePosition, scaleWithZoom);
                }
            }

            FRect dest = new(basePosition.X, basePosition.Y, glyph.SourceRect.w * scaleWithZoom, glyph.SourceRect.h * scaleWithZoom);
            Font.Texture.Render(renderer, glyph.SourceRect, dest);

            cursorX += glyph.XAdvance * scaleWithZoom;
        }
    }

    public void SetFontSize(int fontSize)
    {
        FontSize = fontSize / Font.FontData.Config.CharHeight;
    }

    public void SetText(string text)
    {
        if (Content == text)
        {
            return;
        }
        Content = text;
    }
}