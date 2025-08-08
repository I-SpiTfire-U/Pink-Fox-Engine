using SDL;

namespace PinkFox.Core.Scenes;

public static class SceneManager
{
    private static readonly Dictionary<string, IScene> _Scenes = [];
    private static readonly Stack<IScene> _SceneStack = new();

    public static void RegisterScene(string name, IScene scene)
    {
        if (!_Scenes.ContainsKey(name))
        {
            _Scenes[name] = scene;
        }
    }

    public static void UnregisterScene(string name)
    {
        if (_Scenes.TryGetValue(name, out var scene))
        {
            scene.Dispose();
            _Scenes.Remove(name);
        }
    }

    public static void SwitchToScene(string name, bool disposeCurrent = true)
    {
        if (!_Scenes.TryGetValue(name, out IScene? scene))
        {
            throw new ArgumentException($"Scene '{name}' not registered.");
        }

        if (_SceneStack.Count > 0)
        {
            IScene current = _SceneStack.Pop();
            if (disposeCurrent)
            {
                current.Dispose();
            }
        }

        _SceneStack.Push(scene);
        
        if (!scene.HasBeenLoaded)
        {
            scene.LoadContent();
        }
    }

    public static void PushScene(string name)
    {
        if (!_Scenes.TryGetValue(name, out IScene? scene))
        {
            Console.WriteLine($"[SceneManager] Scene '{name}' not found.");
            return;
        }
        
        _SceneStack.Push(scene);

        if (!scene.HasBeenLoaded)
        {
            scene.LoadContent();
        }
    }

    public static void PopScene(bool dispose = true)
    {
        if (_SceneStack.Count == 0)
        {
            return;
        }

        IScene scene = _SceneStack.Pop();

        if (dispose)
        {
            scene.Dispose();
        }
    }

    public static IScene? GetActiveScene()
    {
        return _SceneStack.TryPeek(out var scene) ? scene : null;
    }

    public static void Update(float deltaTime)
    {
        if (_SceneStack.TryPeek(out IScene? scene))
        {
            scene.Update(deltaTime);
        }
    }

    public static void FixedUpdate(float fixedUpdateInterval)
    {
        if (_SceneStack.TryPeek(out IScene? scene))
        {
            scene.FixedUpdate(fixedUpdateInterval);
        }
    }

    public static unsafe void Draw(SDL_Renderer* renderer)
    {
        if (_SceneStack.TryPeek(out IScene? scene))
        {
            scene.Draw(renderer);
        }
    }

    public static void ClearAllScenes()
    {
        while (_SceneStack.Count > 0)
        {
            IScene scene = _SceneStack.Pop();
            scene.Dispose();
        }

        _Scenes.Clear();
    }
}