using SadConsole;
using SadRogue.Primitives;

namespace MIST
{
    internal class ScreenContainer : ScreenObject
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static ScreenContainer Instance { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public readonly GameObject Player;
        public readonly Map Map;
        public readonly int Width, Height;

        public List<GameObject> Objects;
        public readonly Random Random;

        public UI UI { get; set; }

        public ScreenContainer()
        {
            Instance = this;
            Width = Constants.ScreenWidth;
            Height = Constants.ScreenHeight;
            Objects = new List<GameObject>();
            Random = new Random();
            UI = new UI(this);
            // generate the map
            Map = Map.GenerateMap(Width, Height, Objects, UI);
            Map.IsFocused = true;

            // add the player
            Player = new GameObject(new ColoredGlyph(Color.White, Color.Black, '@'), Map.start, Map, new Fighter(30, 30, 3, 3, UI), new Info("Player", "It's you!", monsterType.player), null, UI);
            Objects.Add(Player);
            Map.UpdateFOV(Player.Position.X, Player.Position.Y, 5);
            Map.Draw();
            Player.Draw();
            UI.SendMessage("Welcome! this is a test message.");
            UI.Draw(Player);

            Children.Add(Map);
        }
    }
}
