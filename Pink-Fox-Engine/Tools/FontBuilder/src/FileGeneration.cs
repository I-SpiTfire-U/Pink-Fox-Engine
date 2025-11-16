using System.IO.Compression;
using System.Text.Json;
using PinkFox.UI.Fonts;
using SkiaSharp;

namespace PinkFox.Tools.FontBuilder;

public static class FileGeneration
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };
    
    public static void CreateAtlasFile(string atlasPath, SKBitmap atlas)
    {
        using SKImage image = SKImage.FromBitmap(atlas);
        using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
        using FileStream stream = File.OpenWrite(atlasPath);
        data.SaveTo(stream);
    }

    public static void GenerateJsonFile(string jsonPath, BitmapFontData data)
    {
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(data, JsonSerializerOptions));
    }

    public static void CreateFontArchive(string tempDirectory, string fontName, string atlasPath, string jsonPath)
    {
        string outputPath = Path.Combine("fonts", "out");
        CreateOutputDirectory(outputPath);

        string archivePath = Path.Combine(outputPath, $"{fontName}.font");

        using ZipArchive zip = ZipFile.Open(archivePath, ZipArchiveMode.Create);
        zip.CreateEntryFromFile(atlasPath, $"{fontName}.png");
        zip.CreateEntryFromFile(jsonPath, $"{fontName}.json");

        DeleteTempDirectory(tempDirectory, atlasPath, jsonPath);
    }

    public static void DeleteExistingArchive(string archivePath)
    {
        if (File.Exists(archivePath))
        {
            File.Delete(archivePath);
        }
    }

    private static void CreateOutputDirectory(string outputPath)
    {
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
    }

    private static void DeleteTempDirectory(string tempDirectory, string atlasPath, string jsonPath)
    {
        File.Delete(atlasPath);
        File.Delete(jsonPath);
        Directory.Delete(tempDirectory);
    }
}