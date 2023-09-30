using SadConsole;
using SadRogue.Primitives;

namespace MIST.items.consumables
{
    public class cookedbeef : IConsumableItem
    {
        public ItemType Type => ItemType.Consumable;

        public Info Info => new Info("cooked beef", "a cooked bovine beef. YUM!");

        private GameObject ThisObject;
        public GameObject? Object { get => ThisObject; set => ThisObject = value; }

        public cookedbeef(UI ui, IScreenSurface surface)
        {
            ThisObject = new GameObject(new ColoredGlyph(Color.Orange, Color.Black, '*'), new Point(0, 0), surface, null, Info, null, ui);
        }

        public void Use(UI ui, GameObject player)
        {
            ui.SendMessage("You eat the beef. it's delicious.");

            var rng = new Random();
            // heal the player a little bit
            player.Fighter.HP = Math.Min(player.Fighter.maxHP, player.Fighter.HP + rng.Next(10));

            // destroy the object
            ui.playerinventory.Remove(this);
        }
    }
}