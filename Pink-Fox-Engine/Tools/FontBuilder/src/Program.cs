using System.IO.Compression;
using System.Text.Json;
using PinkFox.Graphics.Fonts;
using SkiaSharp;

namespace PinkFox.Tools.FontBuilder;

public class Program
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public static void Main(string[] arguments)
    {
        int fontSize = 32;
        if (arguments.Length > 0)
        {
            fontSize = int.Parse(arguments[0]);
        }

        string[] fontsToConvert = Directory.GetFiles("fonts");

        if (fontsToConvert.Length == 0)
        {
            Console.WriteLine("No fonts to convert.");
            return;
        }

        foreach (string font in fontsToConvert)
        {
            string name = Path.GetFileNameWithoutExtension(font);
            string outputDirectory = Path.Combine("fonts", "out", name);
            ImportTTF(font, outputDirectory, fontSize, name);
        }
    }

    public static void ImportTTF(string fontPath, string outputDirectory, int fontSize = 32, string fontName = "CustomFont")
    {
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        SKTypeface typeface = SKTypeface.FromFile(fontPath);
        SKFont font = new(typeface, fontSize);
        SKBitmap atlasBitmap = new(1024, 1024);

        List<Symbol> symbols = GetSymbols(font, atlasBitmap);
        List<Kerning> kernings = GetKernings(font);

        FontConfig fontConfig = new()
        {
            FontFace = fontName,
            CharHeight = fontSize,
            LineSpacing = 0,
            BaseLine = fontSize,
            Size = fontSize,
            TextureFile = $"{fontName}.png",
            TextureWidth = atlasBitmap.Width,
            TextureHeight = atlasBitmap.Height,
            CharSpacing = 0,
            Bold = 0,
            Italic = 0,
            Smooth = 1
        };

        BitmapFontData fontData = new()
        {
            Config = fontConfig,
            Symbols = symbols,
            Kerning = kernings
        };

        string atlasPath = GenerateAtlasFile(outputDirectory, fontName, atlasBitmap);
        string jsonPath = GenerateJsonFile(outputDirectory, fontName, fontData);
        string archivePath = GenerateFontArchive(outputDirectory, fontName, atlasPath, jsonPath);

        Console.WriteLine($"Font imported successfully: {fontName}");
        Console.WriteLine($"Path: {archivePath}");
    }

    private static List<Symbol> GetSymbols(SKFont font, SKBitmap atlasBitmap)
    {
        List<Symbol> result = [];
        SKPaint paint = new()
        {
            IsAntialias = true,
            IsStroke = false,
            Color = SKColors.White
        };

        using SKCanvas canvas = new(atlasBitmap);
        canvas.Clear(SKColors.Transparent);

        int padding = 2;
        int cursorX = padding;
        int cursorY = padding;
        int rowHeight = 0;

        for (char c = ' '; c < 127; c++)
        {
            string str = c.ToString();
            if (string.IsNullOrWhiteSpace(str))
            {
                continue;
            }

            float glyphHeight = font.MeasureText(c.ToString(), out SKRect bounds);

            int w = (int)Math.Ceiling(bounds.Width);
            int h = (int)Math.Ceiling(bounds.Height);

            if (cursorX + w + padding > atlasBitmap.Width)
            {
                cursorX = padding;
                cursorY += rowHeight + padding;
                rowHeight = 0;
            }

            float drawX = cursorX - bounds.Left;
            float drawY = cursorY - bounds.Top;
            canvas.DrawText(str, drawX, drawY, font, paint);

            rowHeight = Math.Max(rowHeight, h + padding);

            float baseline = font.Metrics.Ascent;

            result.Add(new Symbol
            {
                Id = c,
                X = cursorX,
                Y = cursorY,
                Width = w,
                Height = h,
                XOffset = (int)Math.Round(bounds.Left),
                YOffset = (int)Math.Round(baseline - bounds.Top),
                XAdvance = (int)Math.Ceiling(glyphHeight)
            });

            cursorX += w + padding;
        }


        return result;
    }

    private static List<Kerning> GetKernings(SKFont font)
    {
        List<Kerning> result = [];

        for (char first = ' '; first < 127; first++)
        {
            for (char second = ' '; second < 127; second++)
            {
                float width1 = font.MeasureText(first.ToString());
                float width2 = font.MeasureText(second.ToString());
                float pairWidth = font.MeasureText($"{first}{second}");
                int kernAmount = (int)Math.Round(pairWidth - (width1 + width2));

                if (kernAmount != 0)
                {
                    result.Add(new Kerning()
                    {
                        First = first,
                        Second = second,
                        Amount = kernAmount
                    });
                }
            }
        }

        return result;
    }

    private static string GenerateAtlasFile(string outputDirectory, string fontName, SKBitmap atlas)
    {
        string atlasPath = Path.Combine(outputDirectory, $"{fontName}.png");
        using SKImage image = SKImage.FromBitmap(atlas);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using FileStream stream = File.OpenWrite(atlasPath);
        data.SaveTo(stream);

        return atlasPath;
    }

    private static string GenerateJsonFile(string outputDirectory, string fontName, BitmapFontData data)
    {
        string jsonPath = Path.Combine(outputDirectory, $"{fontName}.json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(data, JsonSerializerOptions));

        return jsonPath;
    }

    private static string GenerateFontArchive(string outputDirectory, string fontName, string atlasPath, string jsonPath)
    {
        string outDirectory = Path.Combine("fonts", "out");
        if (!Directory.Exists(outDirectory))
        {
            Directory.CreateDirectory(outDirectory);
        }

        string outputPath = Path.Combine(outDirectory, $"{fontName}.font");

        if (File.Exists(outputPath))
        {
            File.Delete(outputPath);
        }

        using ZipArchive zip = ZipFile.Open(outputPath, ZipArchiveMode.Create);
        zip.CreateEntryFromFile(atlasPath, $"{fontName}.png");
        zip.CreateEntryFromFile(jsonPath, $"{fontName}.json");

        File.Delete(atlasPath);
        File.Delete(jsonPath);
        Directory.Delete(outputDirectory);

        return outputPath;
    }
}