using System.Text.Json;

namespace PinkFox.Graphics.Fonts;

public class BitmapFontData
{
    public required FontConfig Config { get; set; }
    public required List<Kerning> Kerning { get; set; }
    public required List<Symbol> Symbols { get; set; }
    
    public static BitmapFontData FromResource(string resourceName)
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