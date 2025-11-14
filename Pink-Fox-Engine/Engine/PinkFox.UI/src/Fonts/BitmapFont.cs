using PinkFox.Graphics.Rendering;
using SDL;
using PinkFox.Core.Types;

namespace PinkFox.UI.Fonts;

public class BitmapFont : IDisposable
{
    public Texture2D Texture { get; init; }
    public Dictionary<char, GlyphInfo> Characters { get; init; }
    public Dictionary<(char, char), int> KerningPairs { get; init; }
    public BitmapFontData FontData { get; init; }

    public float LineHeight { get; init; }
    public float LineSpacing { get; init; }
    public int SpaceAdvance { get; init; }

    private bool _Disposed;

    public static unsafe BitmapFont FromFontResource(string resourceName, Renderer renderer)
    {
        (nint surface, string jsonText) = Core.ResourceManager.CreateFontArchiveFromResource(resourceName);
        Texture2D texture = new((SDL_Surface*)surface, renderer);
        BitmapFontData fontData = BitmapFontData.FromJson(jsonText);

        return new BitmapFont(texture, fontData);
    }

    public static BitmapFont FromMultipleResources(string textureResourceName, string fontResourceName, Renderer renderer)
    {
        Texture2D texture = Texture2D.FromResource(textureResourceName, renderer);
        BitmapFontData fontData = BitmapFontData.FromResource(fontResourceName);

        return new BitmapFont(texture, fontData);
    }

    public BitmapFont(Texture2D texture, BitmapFontData bitmapFontData)
    {
        Texture = texture;
        FontData = bitmapFontData;

        LineHeight = FontData.Config.CharHeight + FontData.Config.LineSpacing;
        if (SpaceAdvance == 0)
        {
            SpaceAdvance = (int)(LineHeight * 0.33f);
        }

        Characters = new(FontData.Symbols.Count);
        KerningPairs = new(FontData.Kerning.Count);

        foreach (Symbol symbol in FontData.Symbols)
        {
            char c = (char)symbol.Id;
            Characters[c] = new GlyphInfo
            {
                SourceRect = new SDL_FRect
                {
                    x = symbol.X,
                    y = symbol.Y,
                    w = symbol.Width,
                    h = symbol.Height
                },
                XOffset = symbol.XOffset,
                YOffset = symbol.YOffset,
                XAdvance = symbol.XAdvance
            };

            if (c == ' ')
            {
                SpaceAdvance = symbol.XAdvance;
            }
        }

        foreach (Kerning kerning in FontData.Kerning)
        {
            KerningPairs[((char)kerning.First, (char)kerning.Second)] = kerning.Amount;
        }
    }

    public void Dispose()
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
            Texture.Dispose();
            Characters.Clear();
            KerningPairs.Clear();
        }

        _Disposed = true;
    }

    ~BitmapFont()
    {
        Dispose(false);
    }
}