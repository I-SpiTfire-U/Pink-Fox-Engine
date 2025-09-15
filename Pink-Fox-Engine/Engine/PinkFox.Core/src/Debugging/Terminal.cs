using System.Diagnostics;

namespace PinkFox.Core.Debugging;

public static class Terminal
{
    private const string Reset = "\u001b[0m";
    private const string Green = "\u001b[92m";
    private const string Cyan = "\u001b[96m";
    private const string Yellow = "\u001b[93m";
    private const string Red = "\u001b[91m";

    private static readonly Dictionary<LogLevel, string> LevelNames = new()
    {
        { LogLevel.None,        "NONE"        },
        { LogLevel.Success,     "SUCCESS"     },
        { LogLevel.Information, "INFORMATION" },
        { LogLevel.Warning,     "WARNING"     },
        { LogLevel.Error,       "ERROR"       }
    };

    private static string GetAnsiColor(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Success     => Green,
            LogLevel.Information => Cyan,
            LogLevel.Warning     => Yellow,
            LogLevel.Error       => Red,
            _                    => Reset
        };
    }

    [Conditional("DEBUG")]
    public static void LogMessage(LogLevel logLevel, string message)
    {
        string levelText = LevelNames[logLevel];
        string color = GetAnsiColor(logLevel);

        Console.WriteLine($"[{color}{levelText}{Reset}] {message}");
    }
}