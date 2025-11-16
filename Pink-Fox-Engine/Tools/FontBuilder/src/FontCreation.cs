using PinkFox.UI.Fonts;
using SkiaSharp;

namespace PinkFox.Tools.FontBuilder;

public static class FontCreation
{
    public static void CreateFontArchive(string fontPath, string tempDirectory, int fontSize, string fontName)
    {
        if (!Directory.Exists(tempDirectory))
        {
            Directory.CreateDirectory(tempDirectory);
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

        string atlasPath = PathCreation.ConstructAtlasPath(tempDirectory, fontName);
        string jsonPath = PathCreation.ConstructJsonPath(tempDirectory, fontName);
        string archivePath = PathCreation.ConstructArchivePath(tempDirectory, fontName);

        FileGeneration.CreateAtlasFile(atlasPath, atlasBitmap);
        FileGeneration.GenerateJsonFile(jsonPath, fontData);
        FileGeneration.CreateFontArchive(tempDirectory, fontName, atlasPath, jsonPath);

        FileGeneration.DeleteExistingArchive(tempDirectory);

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

            float RenderX = cursorX - bounds.Left;
            float RenderY = cursorY - bounds.Top;
            canvas.DrawText(str, RenderX, RenderY, font, paint);

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
}