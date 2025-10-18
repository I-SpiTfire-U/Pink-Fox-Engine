using SDL;

namespace PinkFox.Graphics;

public readonly struct Color
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(Colors colorEnum)
    {
        uint hex = (uint)colorEnum;

        R = (byte)((hex & 0xFF000000) >> 24);
        G = (byte)((hex & 0x00FF0000) >> 16);
        B = (byte)((hex & 0x0000FF00) >> 8);
        A = (byte)(hex & 0x000000FF);
    }

    public Color(string hex)
    {
        if (hex.StartsWith('#'))
        {
            hex = hex[1..];
        }

        R = Convert.ToByte(hex[..2], 16);
        G = Convert.ToByte(hex.Substring(2, 2), 16);
        B = Convert.ToByte(hex.Substring(4, 2), 16);
        A = (hex.Length >= 8) ? Convert.ToByte(hex.Substring(6, 2), 16) : (byte)255;
    }

    public static implicit operator SDL_Color(Color c) => new() { r = c.R, g = c.G, b = c.B, a = c.A };
    public static implicit operator Color(SDL_Color c) => new(c.r, c.g, c.b, c.a);
    public static implicit operator Color(Colors colorEnum) => new(colorEnum);

    public Color WithAlpha(byte a) => new(R, G, B, a);

    public static Color Lerp(Color c1, Color c2, float t)
    {
        t = Math.Clamp(t, 0f, 1f);

        byte r = (byte)(c1.R + (c2.R - c1.R) * t);
        byte g = (byte)(c1.G + (c2.G - c1.G) * t);
        byte b = (byte)(c1.B + (c2.B - c1.B) * t);
        byte a = (byte)(c1.A + (c2.A - c1.A) * t);

        return new Color(r, g, b, a);
    }

    public override string ToString() => $"Color(R:{R}, G:{G}, B:{B}, A:{A})";

    public static Color White => new(Colors.White);
    public static Color Black => new(Colors.Black);
    public static Color Red => new(Colors.Red);
    public static Color Green => new(Colors.Green);
    public static Color Blue => new(Colors.Blue);
    public static Color Yellow => new(Colors.Yellow);
    public static Color Cyan => new(Colors.Cyan);
    public static Color Magenta => new(Colors.Magenta);
    public static Color Transparent => new(Colors.Transparent);

    public static Color AliceBlue => new(Colors.AliceBlue);
    public static Color AntiqueWhite => new(Colors.AntiqueWhite);
    public static Color Aqua => new(Colors.Aqua);
    public static Color Aquamarine => new(Colors.Aquamarine);
    public static Color Azure => new(Colors.Azure);
    public static Color Beige => new(Colors.Beige);
    public static Color Bisque => new(Colors.Bisque);
    public static Color BlanchedAlmond => new(Colors.BlanchedAlmond);
    public static Color BlueViolet => new(Colors.BlueViolet);
    public static Color Brown => new(Colors.Brown);
    public static Color BurlyWood => new(Colors.BurlyWood);
    public static Color CadetBlue => new(Colors.CadetBlue);
    public static Color Chartreuse => new(Colors.Chartreuse);
    public static Color Chocolate => new(Colors.Chocolate);
    public static Color Coral => new(Colors.Coral);
    public static Color CornflowerBlue => new(Colors.CornflowerBlue);
    public static Color Cornsilk => new(Colors.Cornsilk);
    public static Color Crimson => new(Colors.Crimson);
    public static Color DarkBlue => new(Colors.DarkBlue);
    public static Color DarkCyan => new(Colors.DarkCyan);
    public static Color DarkGoldenrod => new(Colors.DarkGoldenrod);
    public static Color DarkGray => new(Colors.DarkGray);
    public static Color DarkGreen => new(Colors.DarkGreen);
    public static Color DarkKhaki => new(Colors.DarkKhaki);
    public static Color DarkMagenta => new(Colors.DarkMagenta);
    public static Color DarkOliveGreen => new(Colors.DarkOliveGreen);
    public static Color DarkOrange => new(Colors.DarkOrange);
    public static Color DarkOrchid => new(Colors.DarkOrchid);
    public static Color DarkRed => new(Colors.DarkRed);
    public static Color DarkSalmon => new(Colors.DarkSalmon);
    public static Color DarkSeaGreen => new(Colors.DarkSeaGreen);
    public static Color DarkSlateBlue => new(Colors.DarkSlateBlue);
    public static Color DarkSlateGray => new(Colors.DarkSlateGray);
    public static Color DarkTurquoise => new(Colors.DarkTurquoise);
    public static Color DarkViolet => new(Colors.DarkViolet);
    public static Color DeepPink => new(Colors.DeepPink);
    public static Color DeepSkyBlue => new(Colors.DeepSkyBlue);
    public static Color DimGray => new(Colors.DimGray);
    public static Color DodgerBlue => new(Colors.DodgerBlue);
    public static Color Firebrick => new(Colors.Firebrick);
    public static Color FloralWhite => new(Colors.FloralWhite);
    public static Color ForestGreen => new(Colors.ForestGreen);
    public static Color Fuchsia => new(Colors.Fuchsia);
    public static Color GhostWhite => new(Colors.GhostWhite);
    public static Color Gold => new(Colors.Gold);
    public static Color Goldenrod => new(Colors.Goldenrod);
    public static Color Gray => new(Colors.Gray);
    public static Color GreenYellow => new(Colors.GreenYellow);
    public static Color Honeydew => new(Colors.Honeydew);
    public static Color HotPink => new(Colors.HotPink);
    public static Color IndianRed => new(Colors.IndianRed);
    public static Color Indigo => new(Colors.Indigo);
    public static Color Ivory => new(Colors.Ivory);
    public static Color Khaki => new(Colors.Khaki);
    public static Color Lavender => new(Colors.Lavender);
    public static Color LavenderBlush => new(Colors.LavenderBlush);
    public static Color LawnGreen => new(Colors.LawnGreen);
    public static Color LemonChiffon => new(Colors.LemonChiffon);
    public static Color LightBlue => new(Colors.LightBlue);
    public static Color LightCoral => new(Colors.LightCoral);
    public static Color LightCyan => new(Colors.LightCyan);
    public static Color LightGoldenrodYellow => new(Colors.LightGoldenrodYellow);
    public static Color LightGreen => new(Colors.LightGreen);
    public static Color LightGrey => new(Colors.LightGrey);
    public static Color LightPink => new(Colors.LightPink);
    public static Color LightSalmon => new(Colors.LightSalmon);
    public static Color LightSeaGreen => new(Colors.LightSeaGreen);
    public static Color LightSkyBlue => new(Colors.LightSkyBlue);
    public static Color LightSlateGray => new(Colors.LightSlateGray);
    public static Color LightSteelBlue => new(Colors.LightSteelBlue);
    public static Color LightYellow => new(Colors.LightYellow);
    public static Color Lime => new(Colors.Lime);
    public static Color LimeGreen => new(Colors.LimeGreen);
    public static Color Linen => new(Colors.Linen);
    public static Color Maroon => new(Colors.Maroon);
    public static Color MediumAquaMarine => new(Colors.MediumAquaMarine);
    public static Color MediumBlue => new(Colors.MediumBlue);
    public static Color MediumOrchid => new(Colors.MediumOrchid);
    public static Color MediumPurple => new(Colors.MediumPurple);
    public static Color MediumSeaGreen => new(Colors.MediumSeaGreen);
    public static Color MediumSlateBlue => new(Colors.MediumSlateBlue);
    public static Color MediumSpringGreen => new(Colors.MediumSpringGreen);
    public static Color MediumTurquoise => new(Colors.MediumTurquoise);
    public static Color MediumVioletRed => new(Colors.MediumVioletRed);
    public static Color MidnightBlue => new(Colors.MidnightBlue);
    public static Color MintCream => new(Colors.MintCream);
    public static Color MistyRose => new(Colors.MistyRose);
    public static Color Moccasin => new(Colors.Moccasin);
    public static Color NavajoWhite => new(Colors.NavajoWhite);
    public static Color Navy => new(Colors.Navy);
    public static Color OldLace => new(Colors.OldLace);
    public static Color Olive => new(Colors.Olive);
    public static Color OliveDrab => new(Colors.OliveDrab);
    public static Color Orange => new(Colors.Orange);
    public static Color OrangeRed => new(Colors.OrangeRed);
    public static Color Orchid => new(Colors.Orchid);
    public static Color PaleGoldenrod => new(Colors.PaleGoldenrod);
    public static Color PaleGreen => new(Colors.PaleGreen);
    public static Color PaleTurquoise => new(Colors.PaleTurquoise);
    public static Color PaleVioletRed => new(Colors.PaleVioletRed);
    public static Color PapayaWhip => new(Colors.PapayaWhip);
    public static Color PeachPuff => new(Colors.PeachPuff);
    public static Color Peru => new(Colors.Peru);
    public static Color Pink => new(Colors.Pink);
    public static Color Plum => new(Colors.Plum);
    public static Color PowderBlue => new(Colors.PowderBlue);
    public static Color Purple => new(Colors.Purple);
    public static Color RosyBrown => new(Colors.RosyBrown);
    public static Color RoyalBlue => new(Colors.RoyalBlue);
    public static Color SaddleBrown => new(Colors.SaddleBrown);
    public static Color Salmon => new(Colors.Salmon);
    public static Color SandyBrown => new(Colors.SandyBrown);
    public static Color SeaGreen => new(Colors.SeaGreen);
    public static Color Seashell => new(Colors.Seashell);
    public static Color Sienna => new(Colors.Sienna);
    public static Color Silver => new(Colors.Silver);
    public static Color SkyBlue => new(Colors.SkyBlue);
    public static Color SlateBlue => new(Colors.SlateBlue);
    public static Color SlateGray => new(Colors.SlateGray);
    public static Color Snow => new(Colors.Snow);
    public static Color SpringGreen => new(Colors.SpringGreen);
    public static Color SteelBlue => new(Colors.SteelBlue);
    public static Color Tan => new(Colors.Tan);
    public static Color Teal => new(Colors.Teal);
    public static Color Thistle => new(Colors.Thistle);
    public static Color Tomato => new(Colors.Tomato);
    public static Color Turquoise => new(Colors.Turquoise);
    public static Color Violet => new(Colors.Violet);
    public static Color Wheat => new(Colors.Wheat);
    public static Color WhiteSmoke => new(Colors.WhiteSmoke);
    public static Color YellowGreen => new(Colors.YellowGreen);
}