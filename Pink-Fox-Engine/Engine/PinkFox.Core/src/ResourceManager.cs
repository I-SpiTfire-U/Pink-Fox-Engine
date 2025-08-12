using System.Reflection;
using SDL;

namespace PinkFox.Core;

public static class ResourceManager
{
    private static readonly Dictionary<string, byte[]> _ResourcesDictionary = [];

    public static void LoadResources()
    {
        _ResourcesDictionary.Clear();

        Assembly assembly = Assembly.GetEntryAssembly() ?? throw new Exception("Entry assembly not found");

        foreach (string resourcePath in assembly.GetManifestResourceNames())
        {
            string name = resourcePath.Split('.')[^2];

            using Stream? stream = assembly.GetManifestResourceStream(resourcePath) ?? throw new FileNotFoundException($"Embedded resource '{resourcePath}' not found.");

            using MemoryStream ms = new();
            stream.CopyTo(ms);

            byte[] data = ms.ToArray();

            if (_ResourcesDictionary.ContainsKey(name))
            {
                throw new Exception($"Duplicate resource key detected: '{name}'");
            }

            _ResourcesDictionary.Add(name, data);
        }
    }

    public static void ListAllResources()
    {
        foreach (string resourceName in _ResourcesDictionary.Keys)
        {
            Console.WriteLine(resourceName);
        }
    }

    public static byte[] GetResourceByName(string resourceName)
    {
        return _ResourcesDictionary[resourceName];
    }

    public static unsafe SDL_Surface* CreateSurfaceFromResource(string resourceName)
    {
        byte[] data = GetResourceByName(resourceName);
        fixed (byte* pointer = data)
        {
            SDL_IOStream* sdlIoStream = GetIOStream((nint)pointer, (nuint)data.Length);

            SDL_Surface* surface = SDL3_image.IMG_Load_IO(sdlIoStream, true);
            if (surface is null)
            {
                throw new Exception($"Failed to load surface from memory: {SDL3.SDL_GetError()}");
            }

            return surface;
        }
    }

    public static unsafe Mix_Chunk* CreateChunkFromResource(string resourceName)
    {
        byte[] data = GetResourceByName(resourceName);
        fixed (byte* pointer = data)
        {
            SDL_IOStream* sdlIoStream = GetIOStream((nint)pointer, (nuint)data.Length);

            Mix_Chunk* chunk = SDL3_mixer.Mix_LoadWAV_IO(sdlIoStream, true);
            if (chunk is null)
            {
                throw new Exception($"Failed to load sound chunk from memory: {SDL3.SDL_GetError()}");
            }

            return chunk;
        }
    }

    public static unsafe Mix_Music* CreateMusicFromResource(string resourceName)
    {
        byte[] data = GetResourceByName(resourceName);
        fixed (byte* pointer = data)
        {
            SDL_IOStream* sdlIoStream = GetIOStream((nint)pointer, (nuint)data.Length);

            Mix_Music* music = SDL3_mixer.Mix_LoadMUS_IO(sdlIoStream, true);
            if (music is null)
            {
                throw new Exception($"Failed to load music from memory: {SDL3.SDL_GetError()}");
            }

            return music;
        }
    }

    public static unsafe TTF_Font* CreateFontFromResource(string resourceName, float fontSize)
    {
        byte[] data = GetResourceByName(resourceName);
        fixed (byte* pointer = data)
        {
            SDL_IOStream* sdlIoStream = GetIOStream((nint)pointer, (nuint)data.Length);

            TTF_Font* font = SDL3_ttf.TTF_OpenFontIO(sdlIoStream, true, fontSize);
            if (font is null)
            {
                throw new Exception($"Failed to load font from memory: {SDL3.SDL_GetError()}");
            }

            return font;
        }
    }

    public static unsafe string CreateTextFromResource(string resourceName)
    {
        byte[] data = GetResourceByName(resourceName);
        return System.Text.Encoding.UTF8.GetString(data);
    }

    private static unsafe SDL_IOStream* GetIOStream(nint pointer, nuint dataLength)
    {
        SDL_IOStream* sdlIoStream = SDL3.SDL_IOFromConstMem(pointer, dataLength);
        if (sdlIoStream is null)
        {
            throw new Exception($"Failed to create sdlIoStream: {SDL3.SDL_GetError()}");
        }
        return sdlIoStream;
    }
}