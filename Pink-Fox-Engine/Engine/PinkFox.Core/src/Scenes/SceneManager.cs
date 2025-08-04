using SDL;

namespace PinkFox.Core.Scenes;

public static class SceneManager
{
    private static IScene? _ActiveScene;
    private static Action? _ExitAction;

    public static void SetExitAction(Action action)
    {
        if (_ActiveScene is not null && _ExitAction is not null)
        {
            _ActiveScene.OnRequestExit -= _ExitAction;
        }

        _ExitAction = action;

        if (_ActiveScene is not null)
        {
            _ActiveScene.OnRequestExit += _ExitAction;
        }
    }

    public static IScene? GetActiveScene()
    {
        return _ActiveScene;
    }

    public static void LoadScene(IScene scene)
    {
        _ActiveScene?.Dispose();
        
        _ActiveScene = scene;
        _ActiveScene.LoadContent();

        if (_ExitAction is not null)
        {
            _ActiveScene.OnRequestExit += _ExitAction;
        }
    }

    public static void UnloadScene()
    {
        _ActiveScene?.Dispose();
        _ActiveScene = null;
    }

    public static void Update(float deltaTime)
    {
        _ActiveScene?.Update(deltaTime);
    }

    public static void FixedUpdate(float fixedUpdateInterval)
    {
        _ActiveScene?.FixedUpdate(fixedUpdateInterval);
    }

    public static unsafe void Draw(SDL_Renderer* renderer, float alpha)
    {
        _ActiveScene?.Draw(renderer, alpha);
    }
}