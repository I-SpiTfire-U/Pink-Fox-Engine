namespace PinkFox.Graphics.Fonts;

public class BitmapFontData
{
    public required FontConfig Config { get; set; }
    public required List<Kerning> Kerning { get; set; }
    public required List<Symbol> Symbols { get; set; }
}