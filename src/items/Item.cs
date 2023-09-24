using MIST.AI;
using SadConsole;
using SadRogue.Primitives;

namespace MIST.items
{
    internal class Item 
    {
        public ItemType type;

        private static ColoredGlyph itemappearance = new ColoredGlyph(Color.Orange, Color.Black, '*');

        public GameObject? Object { get; set; }
        public Item(Info info, UI ui, ItemType Type, List<Item> items)
        {
            type = Type;

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

            items.Add(this);
        }

        public void SpawnInMap(Map map, UI ui, Point position)
        {
            var item = new GameObject(itemappearance, position, map, null, new Info("Test Item", "This is a test item."), null, ui);
            Object = item;
            
            
        }
    }
}