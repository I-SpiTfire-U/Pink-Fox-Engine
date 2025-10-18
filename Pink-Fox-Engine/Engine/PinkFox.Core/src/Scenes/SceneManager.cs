using PinkFox.Core.Debugging;

namespace PinkFox.Core.Scenes;

public class SceneManager
{
    private readonly Stack<IScene> _SceneStack = new();
    private readonly Dictionary<string, IScene> _Scenes = [];

    public void RegisterScene(string name, IScene scene)
    {
        if (_Scenes.ContainsKey(name))
        {
            Terminal.LogMessage(LogLevel.Warning, $"A scene with the name '{name}' already exists, skipping");
            return;
        }
        _Scenes[name] = scene;
        Terminal.LogMessage(LogLevel.Success, $"The scene '{name}' has been registered");
    }

    public void UnregisterScene(string name)
    {
        if (!_Scenes.TryGetValue(name, out IScene? scene))
        {
            Terminal.LogMessage(LogLevel.Warning, $"A scene with the name '{name}' is not registered, skipping");
            return;
        }
        scene.Dispose();
        _Scenes.Remove(name);
        Terminal.LogMessage(LogLevel.Success, $"The scene '{name}' has been unregistered");
    }

    public void SwitchToScene(string name, bool disposeCurrent = true)
    {
        if (!_Scenes.TryGetValue(name, out IScene? scene))
        {
            Terminal.LogMessage(LogLevel.Error, $"The scene '{name}' is not registered");
            throw new Exception();
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

    public void PushScene(string name)
    {
        if (!_Scenes.TryGetValue(name, out IScene? scene))
        {
            Terminal.LogMessage(LogLevel.Error, $"The scene '{name}' is not registered");
            throw new Exception();
        }
        
        _SceneStack.Push(scene);

        if (!scene.HasBeenLoaded)
        {
            scene.LoadContent();
        }
    }

    public void PopScene(bool dispose = true)
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

    public IScene? GetActiveScene()
    {
        return _SceneStack.TryPeek(out IScene? scene) ? scene : null;
    }

    public IScene? GetScene(string name)
    {
        if (!_Scenes.TryGetValue(name, out IScene? scene))
        {
            Terminal.LogMessage(LogLevel.Error, $"The scene '{name}' is not registered");
            return null;
        }
        return scene;
    }

    public void Update(float deltaTime)
    {
        GetActiveScene()?.Update(deltaTime);
    }

    public void FixedUpdate(float fixedUpdateInterval)
    {
        GetActiveScene()?.FixedUpdate(fixedUpdateInterval);
    }

    public unsafe void Render(float deltaTime)
    {
        GetActiveScene()?.Render(deltaTime);
    }

    public void ClearAllScenes()
    {
        while (_SceneStack.Count > 0)
        {
            IScene scene = _SceneStack.Pop();
            scene.Dispose();
        }

        _Scenes.Clear();
    }
}