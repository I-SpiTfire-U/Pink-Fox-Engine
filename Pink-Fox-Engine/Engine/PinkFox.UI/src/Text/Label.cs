using System.Numerics;
using SDL;

namespace PinkFox.UI.Text;

public class Label : IDisposable
{
    public bool IsTyping { get; private set; } = false;
    public Vector2 Position { get; set; }
    public SDL_Color Color { get; set; } = new()
    {
        r = 255,
        g = 255,
        b = 255,
        a = 255
    };
    public string? Text { get; private set; }
    public event Action? OnCharacterTyped;
    public event Action? OnTypingBegin;
    public event Action? OnTypingEnd;

    private int _Width;
    private int _Height;
    private unsafe SDL_Texture* _Texture;
    private readonly unsafe SDL_Renderer* _Renderer;
    private readonly Font _Font;

    private string? _FullText;
    private int _CurrentCharCount;
    private float _TimeSinceLastChar;
    private float _CharRevealInterval = 0.05f;
    private float _ShakeIntensity = 0f;
    private float _ShakeDuration = 0f;
    private float _ShakeTimer = 0f;

    private readonly Random _Random = new();

    private bool _Disposed;

    public unsafe Label(SDL_Renderer* renderer, Font font, string text, Vector2 position)
    {
        _Renderer = renderer;
        _Font = font;
        Position = position;
        SetText(text);
    }

    public unsafe void SetText(string text)
    {
        if (Text == text)
        {
            return;
        }

        Text = text;

        if (_Texture is not null)
        {
            SDL3.SDL_DestroyTexture(_Texture);
            _Texture = null;
        }

        SDL_Surface* surface = SDL3_ttf.TTF_RenderText_Solid(_Font.Handle, Text, (nuint)Text.Length, Color);
        if (surface is null)
        {
            return;
        }

        _Width = surface->w;
        _Height = surface->h;

        _Texture = SDL3.SDL_CreateTextureFromSurface(_Renderer, surface);
        SDL3.SDL_DestroySurface(surface);
    }

    public unsafe void SetFullText(string text)
    {
        _FullText = text;
        Text = text;

        if (_Texture is not null)
        {
            SDL3.SDL_DestroyTexture(_Texture);
            _Texture = null;
        }

        SDL_Surface* surface = SDL3_ttf.TTF_RenderText_Solid(_Font.Handle, _FullText, (nuint)_FullText.Length, Color);
        if (surface is null)
        {
            return;
        }

        _Width = surface->w;
        _Height = surface->h;

        _Texture = SDL3.SDL_CreateTextureFromSurface(_Renderer, surface);
        SDL3.SDL_DestroySurface(surface);
    }

    public void StartTypingEffect(string fullText, float charRevealInterval = 0.05f)
    {
        _FullText = fullText;
        _CharRevealInterval = charRevealInterval;
        _CurrentCharCount = 0;
        _TimeSinceLastChar = 0f;
        IsTyping = true;
        SetFullText(fullText);
        OnTypingBegin?.Invoke();
    }

    public void Shake(float intensity, float duration = 0f)
    {
        _ShakeIntensity = intensity;
        _ShakeDuration = duration;
        _ShakeTimer = 0f;
    }

    public void FinishTyping()
    {
        if (!IsTyping || _FullText == null)
        {
            return;
        }
        _CurrentCharCount = _FullText.Length;
        SetText(_FullText);
        IsTyping = false;
        OnTypingEnd?.Invoke();
    }

    private unsafe int GetTextWidth(string text)
    {
        int width = 0, height = 0;

        TTF_Text* textObj = SDL3_ttf.TTF_CreateText(_Font.Engine, _Font.Handle, text, (nuint)text.Length);
        if (textObj is null)
        {
            return 0;
        }

        bool result = SDL3_ttf.TTF_GetTextSize(textObj, &width, &height);
        SDL3_ttf.TTF_DestroyText(textObj);

        if (!result)
        {
            return 0;
        }

        return width;
    }

    public void Update(float deltaTime)
    {
        if (!IsTyping || _FullText == null)
        {
            return;
        }

        _TimeSinceLastChar += deltaTime;

        if (_TimeSinceLastChar >= _CharRevealInterval)
        {
            if (_CurrentCharCount < _FullText.Length && _FullText[_CurrentCharCount] != ' ')
            {
                OnCharacterTyped?.Invoke();
            }

            _TimeSinceLastChar = 0f;
            _CurrentCharCount++;

            if (_CurrentCharCount >= _FullText.Length)
            {
                _CurrentCharCount = _FullText.Length;
                IsTyping = false;
                OnTypingEnd?.Invoke();
            }
        }

        if (_ShakeDuration > 0f)
        {
            _ShakeTimer += deltaTime;
            if (_ShakeTimer >= _ShakeDuration)
            {
                _ShakeIntensity = 0f;
                _ShakeDuration = 0f;
            }
        }
    }

    public unsafe void Draw()
    {
        if (_Texture is null || _FullText is null || _CurrentCharCount == 0)
        {
            return;
        }

        float offsetX = 0f;
        float offsetY = 0f;
        if (_ShakeIntensity > 0)
        {
            offsetX = (float)(_Random.NextDouble() * 2 - 1) * _ShakeIntensity;
            offsetY = (float)(_Random.NextDouble() * 2 - 1) * _ShakeIntensity;
        }

        int clipWidth = GetTextWidth(_FullText[.._CurrentCharCount]);

        SDL_FRect sourceRect = new()
        {
            x = 0,
            y = 0,
            w = clipWidth,
            h = _Height
        };

        SDL_FRect destinationRect = new()
        {
            x = Position.X + offsetX,
            y = Position.Y + offsetY,
            w = clipWidth,
            h = _Height
        };

        SDL3.SDL_RenderTexture(_Renderer, _Texture, &sourceRect, &destinationRect);
    }

    public unsafe void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_Disposed)
        {
            return;
        }
        
        if (disposing)
        {
            unsafe
            {
                if (_Texture is not null)
                {
                    SDL3.SDL_DestroyTexture(_Texture);
                    _Texture = null;
                }
            }
        }

        _Disposed = true;
    }
}
