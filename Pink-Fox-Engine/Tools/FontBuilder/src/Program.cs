namespace PinkFox.Tools.FontBuilder;

public class Program
{
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
            FontCreation.CreateFontArchive(font, outputDirectory, fontSize, name);
        }
    }
}