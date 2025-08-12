using System.Text.Json.Serialization;

namespace PinkFox.Graphics.Fonts;

public class FontConfig
{
    [JsonPropertyName("base")]
    public int BaseLine { get; set; }
    public int Bold { get; set; }
    public int CharHeight { get; set; }
    public int CharSpacing { get; set; }
    [JsonPropertyName("face")]
    public required string FontFace { get; set; }
    public int Italic { get; set; }
    public int LineSpacing { get; set; }
    public int Size { get; set; }
    public int Smooth { get; set; }
    public required string TextureFile { get; set; }
    public int TextureWidth { get; set; }
    public int TextureHeight { get; set; }
}