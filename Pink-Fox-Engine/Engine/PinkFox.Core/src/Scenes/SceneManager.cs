using PinkFox.Core.Interfaces;

namespace PinkFox.Core.Scenes;

public static class SceneManager
{
    public static IScene? ActiveScene;
    private static Action? _ExitAction;

    public static void SetExitAction(Action action)
    {
        if (ActiveScene is not null && _ExitAction is not null)
        {
            ActiveScene.OnRequestExit -= _ExitAction;
        }

        _ExitAction = action;

        if (ActiveScene is not null)
        {
            ActiveScene.OnRequestExit += _ExitAction;
        }
    }

    public static void LoadScene(IScene scene)
    {
        ActiveScene?.Dispose();
        ActiveScene = scene;
        if (_ExitAction is not null)
        {
            ActiveScene.OnRequestExit += _ExitAction;
        }
    }

    public static void UnloadScene()
    {
        ActiveScene?.Dispose();
        ActiveScene = null;
    }

    public static void Update(float deltaTime)
    {
        ActiveScene?.Update(deltaTime);
    }

    public static void FixedUpdate()
    {
        ActiveScene?.FixedUpdate();
    }

    public static void Draw(nint renderer)
    {
        ActiveScene?.Draw(renderer);
    }
}