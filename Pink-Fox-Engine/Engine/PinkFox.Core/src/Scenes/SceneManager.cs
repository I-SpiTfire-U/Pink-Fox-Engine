namespace PinkFox.Core.Scenes;

public static class SceneManager
{
    private static IScene? _ActiveScene;

    public static void LoadScene(IScene scene, nint renderer)
    {
        _ActiveScene = scene;
        _ActiveScene.LoadContent();
    }

    public static void Update(float deltaTime)
    {
        _ActiveScene?.Update(deltaTime);
    }

    public static void FixedUpdate()
    {
        _ActiveScene?.FixedUpdate();
    }

    public static void Draw(nint renderer)
    {
        _ActiveScene?.Draw(renderer);
    }
}