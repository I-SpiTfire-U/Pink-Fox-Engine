using PinkFox.Core.Debugging;
using SDL;

namespace PinkFox.Core;

public static class SdlInitialization
{
    public static bool TryInitializeSdl(SDL_InitFlags initFlags)
    {
        bool result = SDL3.SDL_Init(initFlags);
        if (!result)
        {
            Terminal.LogMessage(LogLevel.Error, "SDL could not be initialized");
            return false;
        }

        Terminal.LogMessage(LogLevel.Success, $"SDL initialized");
        return true;
    }

    public static bool TryInitializeMix(uint sdlMixFlags)
    {
        InitializeMixer(sdlMixFlags);

        SDL_AudioSpec audioSpec = new()
        {
            freq = 44100,
            format = SDL_AudioFormat.SDL_AUDIO_S16LE,
            channels = 2
        };

        if (!TryOpenAudio(audioSpec))
        {
            return false;
        }

        _ = SDL3_mixer.Mix_AllocateChannels(32);

        return TryGetAudioDriver();
    }

    private static void InitializeMixer(uint sdlMixFlags)
    {
        uint result = SDL3_mixer.Mix_Init(sdlMixFlags);

        if ((result & SDL3_mixer.MIX_INIT_MP3) == 0)
        {
            Terminal.LogMessage(LogLevel.Warning, "MP3 format not supported, skipping MP3 initialization");
        }
        if ((result & SDL3_mixer.MIX_INIT_OGG) == 0)
        {
            Terminal.LogMessage(LogLevel.Warning, "OGG format not supported, skipping OGG initialization");
        }
        if ((result & SDL3_mixer.MIX_INIT_WAVPACK) == 0)
        {
            Terminal.LogMessage(LogLevel.Warning, "WAV format not supported, skipping WAV initialization");
        }

        Terminal.LogMessage(LogLevel.Information, $"Mixer Init Result: {result}");
    }

    private static unsafe bool TryOpenAudio(SDL_AudioSpec audioSpec)
    {
        bool result = SDL3_mixer.Mix_OpenAudio(SDL3.SDL_AUDIO_DEVICE_DEFAULT_PLAYBACK, &audioSpec);
        if (!result)
        {
            Terminal.LogMessage(LogLevel.Error, "Mix_OpenAudio failed");
            return false;
        }

        Terminal.LogMessage(LogLevel.Error, "Successfully allocated mixer channels");
        return true;
    }

    private static unsafe bool TryGetAudioDriver()
    {
        string? driver = SDL3.SDL_GetCurrentAudioDriver();
        if (string.IsNullOrEmpty(driver))
        {
            Terminal.LogMessage(LogLevel.Error, "Failed to load audio driver");
            return false;
        }

        Terminal.LogMessage(LogLevel.Information, $"Using SDL audio driver: {driver}");
        return true;
    }
}