using SadConsole;
using SadRogue.Primitives;

namespace MIST.items.consumables
{
    public class healingelixir : IConsumableItem
    {
        public ItemType Type => ItemType.Consumable;

        public Info Info => new Info("healing elixir", "a healing elixir, smells like stawberry.");

        private GameObject ThisObject;
        public GameObject? Object { get => ThisObject; set => ThisObject = value; }

        public healingelixir(UI ui, IScreenSurface surface)
        {
            ThisObject = new GameObject(new ColoredGlyph(Color.Orange, Color.Black, '*'), new Point(0, 0), surface, null, Info, null, ui);
        }

        public void Use(UI ui, GameObject player)
        {
            ui.SendMessage("You drink the elixir. You feel better.");

            // heal the player
            player.Fighter.HP = player.Fighter.maxHP;

            // destroy the object
            ui.playerinventory.Remove(this);
        }
    }
}