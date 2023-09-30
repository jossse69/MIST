using SadConsole;
using SadRogue.Primitives;

namespace MIST.items.consumables
{
    public class bread : IConsumableItem
    {
        public ItemType Type => ItemType.Consumable;

        public Info Info => new Info("bread", "a loaf of bread, that's all.");

        private GameObject ThisObject;
        public GameObject? Object { get => ThisObject; set => ThisObject = value; }

        public bread(UI ui, IScreenSurface surface)
        {
            ThisObject = new GameObject(new ColoredGlyph(Color.Orange, Color.Black, '*'), new Point(0, 0), surface, null, Info, null, ui);
        }

        public void Use(UI ui, GameObject player)
        {
            ui.SendMessage("You eat the bread.");

            // heal the player a little bit
            player.Fighter.HP = Math.Min(player.Fighter.maxHP, player.Fighter.HP + 10);

            // destroy the object
            ui.playerinventory.Remove(this);
        }
    }
}