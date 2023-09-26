using MIST.AI;
using SadConsole;
using SadRogue.Primitives;

namespace MIST.items
{
    public class Item 
    {
        public ItemType type;

        public Info info;

        public UI UI;
        private static ColoredGlyph itemappearance = new ColoredGlyph(Color.Orange, Color.Black, '*');

        public GameObject? Object { get; set; }
        public Item(Info Info, ItemType Type, UI ui, IScreenSurface map, Point position)
        {
            type = Type;
            info = Info;

            if (type == ItemType.Consumable)
            {
                itemappearance = new ColoredGlyph(Color.Orange, Color.Black, '*');
            }
            else if (type == ItemType.Mellee)
            {
                itemappearance = new ColoredGlyph(Color.Orange, Color.Black, '/');
            }
            else if (type == ItemType.Ranged)
            {
                itemappearance = new ColoredGlyph(Color.Orange, Color.Black, ')');
            }
            else if (type == ItemType.Book)
            {
                itemappearance = new ColoredGlyph(Color.Orange, Color.Black, '&');
            }

            var item = new GameObject(itemappearance, position, map, null, info, null, ui);
            Object = item;

        }

        public virtual void Use()
        {
            UI.SendMessage(info.name + " used!");
            // heal player
            var player = ScreenContainer.Instance.Player;
            player.Fighter.HP = Math.Min(player.Fighter.maxHP, player.Fighter.HP + 10);
        }

        public virtual int weponattack(){

            return 4;
        }

    }
}