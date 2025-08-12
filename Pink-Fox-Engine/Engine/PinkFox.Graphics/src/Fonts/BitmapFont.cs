using PinkFox.Graphics.Rendering;
using System.Text.Json;
using SDL;

namespace PinkFox.Graphics.Fonts;

public class BitmapFont
{
    public Texture2D Texture { get; init; }
    public Dictionary<char, GlyphInfo> Characters { get; init; }
    public Dictionary<(char, char), int> KerningPairs { get; init; }
    public BitmapFontData FontData { get; init; }

    public float LineHeight { get; private set; } = 0;
    public float LineSpacing { get; private set; } = 0f;
    public int SpaceAdvance { get; private set; } = 0;

    public BitmapFont(Texture2D texture, BitmapFontData data)
    {
        Texture = texture;
        FontData = data;

        LineHeight = FontData.Config.CharHeight + FontData.Config.LineSpacing;
        if (SpaceAdvance == 0)
        {
            SpaceAdvance = (int)(LineHeight * 0.33f);
        }

        Characters = new(FontData.Symbols.Count);
        KerningPairs = new(FontData.Kerning.Count);

        foreach (var symbol in FontData.Symbols)
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

        foreach (var kerning in FontData.Kerning)
        {
            KerningPairs[((char)kerning.First, (char)kerning.Second)] = kerning.Amount;
        }
    }

    public static BitmapFontData LoadFontDataFromJson(string resourceName)
    {
        string jsonText = Core.ResourceManager.CreateTextFromResource(resourceName);

        JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        BitmapFontData? fontData = JsonSerializer.Deserialize<BitmapFontData>(jsonText, options) ?? throw new Exception("Failed to deserialize BitmapFontData from JSON.");

        return fontData;
    }
}