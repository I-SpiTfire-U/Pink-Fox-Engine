using System.Numerics;
using PinkFox.Core.Modules.Graphics;
using PinkFox.Core.Types;
using SDL;

namespace PinkFox.Graphics.Fonts;

public class Label
{
    private float _Scale;
    private readonly BitmapFont _Font;

    public Vector2 Position { get; set; }
    public string? Text { get; private set; }
    public bool IsTyping { get; private set; } = false;

    public float CharRevealInterval { get; set; } = 0.05f;
    private int _CurrentCharCount;
    private float _TimeSinceLastChar;

    public float ShakeIntensity { get; set; } = 0f;
    private float _ShakeDuration = 0f;
    private float _ShakeTimer = 0f;

    public event Action? OnCharacterTyped;
    public event Action? OnTypingBegin;
    public event Action? OnTypingEnd;

    public Label(BitmapFont font, string? text = null, int fontSize = 32, Vector2? position = null)
    {
        _Font = font;
        _Scale = (float)fontSize / _Font.FontData.Config.CharHeight;
        if (!string.IsNullOrEmpty(text))
        {
            Text = text;
            _CurrentCharCount = Text.Length;
        }
        if (position is not null)
        {
            Position = position.Value;
        }
    }

    public void Update(float deltaTime)
    {
        UpdateTyping(deltaTime);
        UpdateShake(deltaTime);
    }

    private void UpdateTyping(float deltaTime)
    {
        if (string.IsNullOrEmpty(Text) || !IsTyping)
        {
            return;
        }

        _TimeSinceLastChar += deltaTime;

        if (_TimeSinceLastChar >= CharRevealInterval)
        {
            if (_CurrentCharCount < Text!.Length && Text[_CurrentCharCount] != ' ' && Text[_CurrentCharCount] != '\n')
            {
                OnCharacterTyped?.Invoke();
            }

            _TimeSinceLastChar = 0f;
            _CurrentCharCount++;

            if (_CurrentCharCount >= Text.Length)
            {
                _CurrentCharCount = Text.Length;
                IsTyping = false;
                OnTypingEnd?.Invoke();
            }
        }
    }

    private void UpdateShake(float deltaTime)
    {
        if (ShakeIntensity <= 0f)
        {
            return;
        }

        if (_ShakeDuration < 0f)
        {
            return;
        }

        _ShakeTimer += deltaTime;
        if (_ShakeTimer >= _ShakeDuration)
        {
            ShakeIntensity = 0f;
            _ShakeDuration = 0f;
            _ShakeTimer = 0f;
        }
    }

    public unsafe void Draw(Renderer renderer, ICamera2D? camera2D = null, float maxWidth = float.MaxValue)
    {
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        Vector2 screenPosition = camera2D?.WorldToScreen(Position) ?? Position;
        float zoom = camera2D?.Zoom ?? 1f;

        float cursorX = screenPosition.X;
        float cursorY = screenPosition.Y - _Font.FontData.Config.BaseLine * _Scale * zoom;
        char previousChar = '\0';

        float scaleWithZoom = _Scale * zoom;

        float lineHeight = _Font.LineHeight * scaleWithZoom;
        float spaceAdvance = _Font.SpaceAdvance * scaleWithZoom;
        float baseLine = _Font.FontData.Config.BaseLine * scaleWithZoom;

        float currentWidth = 0f;

        for (int i = 0; i < _CurrentCharCount; i++)
        {
            char c = Text[i];

            if (c == '\n')
            {
                cursorX = screenPosition.X;
                cursorY += lineHeight;
                previousChar = '\0';
                currentWidth = 0f;
                continue;
            }

            float charAdvance = 0f;

            if (c == ' ')
            {
                charAdvance = spaceAdvance;
            }
            else if (_Font.Characters.TryGetValue(c, out GlyphInfo? glyphInfo))
            {
                charAdvance = glyphInfo.XAdvance * scaleWithZoom;

                if (previousChar != '\0' && _Font.KerningPairs.TryGetValue((previousChar, c), out int kernAmount))
                {
                    cursorX += kernAmount * scaleWithZoom;
                }
            }
            else
            {
                continue;
            }

            if (currentWidth + charAdvance > maxWidth)
            {
                cursorX = screenPosition.X;
                cursorY += lineHeight;
                currentWidth = 0f;
                previousChar = '\0';
            }

            if (c == ' ')
            {
                cursorX += spaceAdvance;
                currentWidth += spaceAdvance;
                previousChar = '\0';
                continue;
            }

            if (!_Font.Characters.TryGetValue(c, out GlyphInfo? drawGlyph))
            {
                continue;
            }

            if (previousChar != '\0' && _Font.KerningPairs.TryGetValue((previousChar, c), out int kern))
            {
                cursorX += kern * scaleWithZoom;
                currentWidth += kern * scaleWithZoom;
            }

            float drawY = cursorY + baseLine + drawGlyph.YOffset * scaleWithZoom;

            Vector2 shakeOffset = GetShakeOffset();
            FRect destRect = new(cursorX + drawGlyph.XOffset * scaleWithZoom + shakeOffset.X, drawY + shakeOffset.Y, drawGlyph.SourceRect.w * scaleWithZoom, drawGlyph.SourceRect.h * scaleWithZoom);

            _Font.Texture.Draw(renderer, drawGlyph.SourceRect, destRect);

            cursorX += drawGlyph.XAdvance * scaleWithZoom;
            previousChar = c;
            currentWidth += drawGlyph.XAdvance * scaleWithZoom;
        }
    }

    public void SetFontSize(int fontSize)
    {
        _Scale = (float)fontSize / _Font.FontData.Config.CharHeight;
    }

    private Vector2 GetShakeOffset()
    {
        if (ShakeIntensity <= 0f)
        {
            return Vector2.Zero;
        }

        float offsetX = (float)(Random.Shared.NextDouble() * 2 - 1) * 2f * ShakeIntensity * _Scale;
        float offsetY = (float)(Random.Shared.NextDouble() * 2 - 1) * 2f * ShakeIntensity * _Scale;

        return new Vector2(offsetX, offsetY);
    }

    public void SetText(string text)
    {
        if (Text == text)
        {
            return;
        }
        Text = text;
        _CurrentCharCount = Text.Length;
        IsTyping = false;
    }

    public void StartTypingEffect(string text, float charRevealInterval = 0.05f)
    {
        CharRevealInterval = charRevealInterval;
        _CurrentCharCount = 0;
        _TimeSinceLastChar = 0f;
        IsTyping = true;
        Text = text;
        OnTypingBegin?.Invoke();
    }

    public void Shake(float intensity, float duration = -1f)
    {
        ShakeIntensity = intensity;
        _ShakeDuration = duration;
        _ShakeTimer = 0f;
    }

    public void FinishTyping()
    {
        if (!IsTyping || Text == null)
        {
            return;
        }
        _CurrentCharCount = Text.Length;
        IsTyping = false;
    }

    public void Reset()
    {
        Text = "";
        _CurrentCharCount = 0;

        CharRevealInterval = 0.05f;
        _TimeSinceLastChar = 0f;
        IsTyping = false;

        ShakeIntensity = 0f;
        _ShakeDuration = 0f;
        _ShakeTimer = 0f;

        _Scale = 32f / _Font.FontData.Config.CharHeight;
    }
}