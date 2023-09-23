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
            Map = Map.GenerateMap(Width, Height, Objects);
            Map.IsFocused = true;

            // add the player
            Player = new GameObject(new ColoredGlyph(Color.White, Color.Black, '@'), Map.start, Map, new Fighter(10, 10, 3, 3), new Info("Player", "It's you!", monsterType.player), null);
            Objects.Add(Player);
            Player.Draw();


            Children.Add(Map);
        }
    }
}
