using System.Text.Json;
using System.Text.Json.Serialization;

namespace PinkFox.Graphics.Fonts;

public class BitmapFontData
{
    public required FontConfig Config { get; set; }
    public required List<Kerning> Kerning { get; set; }
    public required List<Symbol> Symbols { get; set; }

    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static BitmapFontData FromJson(string jsonText)
    {
        return JsonSerializer.Deserialize(jsonText, FontJsonContext.Default.BitmapFontData)
            ?? throw new Exception("Failed to deserialize BitmapFontData from JSON.");
    }

    public static BitmapFontData FromResource(string resourceName)
    {
        string jsonText = Core.ResourceManager.CreateTextFromResource(resourceName);

        return JsonSerializer.Deserialize(jsonText, FontJsonContext.Default.BitmapFontData)
            ?? throw new Exception("Failed to deserialize BitmapFontData from JSON.");
    }
}

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(BitmapFontData))]
[JsonSerializable(typeof(FontConfig))]
[JsonSerializable(typeof(Kerning))]
[JsonSerializable(typeof(Symbol))]
internal partial class FontJsonContext : JsonSerializerContext;
