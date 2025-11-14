using System.Numerics;
using PinkFox.Core.Modules.Graphics;
using PinkFox.Core.Types;
using PinkFox.UI.Effects;
using PinkFox.UI.Fonts;
using PinkFox.UI.Interfaces;

namespace PinkFox.UI.Types;

public class AnimatedLabel : ILabel
{
    public int FontSize { get; set; }
    public Vector2 Position { get; set; }
    public BitmapFont Font { get; }
    public EffectPool Effects { get; } = new();

    public string? Content { get; private set; }
    public bool IsTyping { get; private set; } = false;

    public float CharRevealInterval { get; set; } = 0.05f;
    private int _CurrentCharCount;
    private float _TimeSinceLastChar;

    public event Action? OnCharacterTyped;
    public event Action? OnTypingBegin;
    public event Action? OnTypingEnd;

    public AnimatedLabel(BitmapFont font, string? text = null, int fontSize = 32, Vector2? position = null)
    {
        Font = font;
        FontSize = fontSize / Font.FontData.Config.CharHeight;
        if (!string.IsNullOrEmpty(text))
        {
            Content = text;
            _CurrentCharCount = Content.Length;
        }
        if (position is not null)
        {
            Position = position.Value;
        }
    }

    public void Update(float deltaTime)
    {
        UpdateTyping(deltaTime);
        Effects.Update(deltaTime);
    }

    private void UpdateTyping(float deltaTime)
    {
        if (string.IsNullOrEmpty(Content) || !IsTyping)
        {
            return;
        }

        _TimeSinceLastChar += deltaTime;

        if (_TimeSinceLastChar >= CharRevealInterval)
        {
            if (_CurrentCharCount < Content!.Length && Content[_CurrentCharCount] != ' ' && Content[_CurrentCharCount] != '\n')
            {
                OnCharacterTyped?.Invoke();
            }

            _TimeSinceLastChar = 0f;
            _CurrentCharCount++;

            if (_CurrentCharCount >= Content.Length)
            {
                _CurrentCharCount = Content.Length;
                IsTyping = false;
                OnTypingEnd?.Invoke();
            }
        }
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

        for (int i = 0; i < _CurrentCharCount; i++)
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
        _CurrentCharCount = Content.Length;
        IsTyping = false;
    }

    public void StartTypingEffect(string text, float charRevealInterval = 0.05f)
    {
        CharRevealInterval = charRevealInterval;
        _CurrentCharCount = 0;
        _TimeSinceLastChar = 0f;
        IsTyping = true;
        Content = text;
        OnTypingBegin?.Invoke();
    }

    public void FinishTyping()
    {
        if (!IsTyping || Content == null)
        {
            return;
        }
        _CurrentCharCount = Content.Length;
        IsTyping = false;
    }

    public void Reset()
    {
        Content = "";
        _CurrentCharCount = 0;

        CharRevealInterval = 0.05f;
        _TimeSinceLastChar = 0f;
        IsTyping = false;
    }
}