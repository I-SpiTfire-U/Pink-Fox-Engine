namespace PinkFox.GameTemplate.Main;

public class Program
{
    [STAThread]
    public static void Main()
    {
        using Game game = new Game();
        game.Initialize();
        game.Run();
    }
}