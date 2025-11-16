namespace PinkFox.Tools.FontBuilder;

public static class PathCreation
{
    public static string ConstructAtlasPath(string outputDirectory, string fontName)
    {
        return Path.Combine(outputDirectory, $"{fontName}.png");
    }

    public static string ConstructJsonPath(string outputDirectory, string fontName)
    {
        return Path.Combine(outputDirectory, $"{fontName}.json");
    }

    public static string ConstructArchivePath(string outputDirectory, string fontName)
    {
        return Path.Combine(outputDirectory, $"{fontName}.json");
    }
}