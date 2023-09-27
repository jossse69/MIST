using MIST.items;
using SadConsole;
using SadRogue.Primitives;

namespace MIST
{
    public class ScreenContainer : ScreenObject
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public static ScreenContainer Instance { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public readonly GameObject Player;
        public readonly Map Map;
        public readonly int Width, Height;

        public List<GameObject> Objects;
        public readonly Random Random;

        public List<items.IItem> Items;

        public UI UI { get; set; }

        public SadConsole.UI.ControlHost controls { get; set; } = new SadConsole.UI.ControlHost();

        public ScreenContainer()
        {
            Instance = this;
            Width = Constants.ScreenWidth;
            Height = Constants.ScreenHeight;
            Objects = new List<GameObject>();
            Random = new Random();
            UI = new UI(this);
            Items = new List<IItem>();  
            Map = Map.GenerateMap(Width, Height, Objects, UI, Items);
            Map.IsFocused = true;

            // add the player
            Player = new GameObject(new ColoredGlyph(Color.White, Color.Black, '@'), Map.start, Map, new Fighter(30, 30, 3, 3, UI, monsterType.player), new Info("Player", "It's you!"), null, UI);
            Objects.Add(Player);
            Map.UpdateFOV(Player.Position.X, Player.Position.Y, 5);
            Map.Draw();
            Player.Draw();
           
            UI.playerinventory.Add(new IItem(new Info("kitchen knife", "a kitchen knife. usable."), ItemType.Mellee, UI, Map, new Point(-1, -1)));
            UI.playerinventory.Add(new IItem(new Info("bread", "a loaf of bread, that's all."), ItemType.Consumable, UI, Map, new Point(-1, -1)));
            UI.SendMessage("Welcome! this is a test message.");
            UI.Draw(Player);

            Children.Add(Map);
        }
    }
}
