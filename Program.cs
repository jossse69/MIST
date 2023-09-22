using SadConsole;

namespace MIST
{
    class Program
    {
        static void Main()
        {
            Settings.WindowTitle = "MIST";

            Game.Configuration gameStartup = new Game.Configuration()
                .SetScreenSize(Constants.ScreenWidth, Constants.ScreenHeight)
                .SetStartingScreen<ScreenContainer>();

            Game.Create(gameStartup);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }
    }
}