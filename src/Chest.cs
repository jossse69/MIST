using MIST.AI;
using MIST.items;
using SadConsole;
using SadRogue.Primitives;

namespace MIST;

public class Chest : GameObject
{
    private static UI ui;
    public List<Item> loot= new List<Item>();

    public bool open = false;
    public Chest(Point position, IScreenSurface surface, Info info, UI UI) : base(new ColoredGlyph(Color.Brown, Color.Black, 234), position, surface, null, info, null, ui)
    {
        loot = new List<Item>();
        ui = UI;
    }

    public static List<Item> GenerateLoot(Map map)
    {
        // fill with 3-4 Consumables, 1-2 Ranged, 1-2 Melee, 1-2 Books
        var rng = new Random();
        var loot = new List<Item>();
        var loottype = rng.Next(5);
        if(loottype < 3)
        {
            for (var i = 0; i < rng.Next(1, 4); i++)
            {
                // 20% chance of a healing elixir, else its chance of a cooked beef.
                if( rng.Next(100) <= 20)
                {
                    loot.Add(new Item(new Info("healing elixir", "a healing elixir. smells sweetly."), ItemType.Consumable, ui, map, new Point(-1, -1)));
                } else
                {
                    loot.Add(new Item(new Info("cooked beef", "a cooked bovine beef. YUM!"), ItemType.Consumable, ui, map, new Point(-1, -1)));
                }
            }
        }
        else if (loottype == 3) {
            for (var i = 0; i < rng.Next(1, 2); i++)
            {
                // 20% chance of a crossbow, else its chance of a bow.
                if (rng .Next(100) <= 20)
                {
                    loot.Add(new Item(new Info("crossbow", "a crossbow. goes pew pew."), ItemType.Ranged, ui, map, new Point(-1, -1)));
                } else
                {
                    loot.Add(new Item(new Info("bow", "a bow. shoots arrows."), ItemType.Ranged, ui, map, new Point(-1, -1)));
                }
            }
        }
        else if (loottype == 4)
        {
            for (var i = 0; i < rng.Next(1, 2); i++)
            {
                // 20% chance of a dagger, else its chance of a sword.
                if (rng .Next(100) <= 20)
                {
                    loot.Add(new Item(new Info("dagger", "a dagger. really sharp."), ItemType.Mellee, ui, map, new Point(-1, -1)));
                }
                else
                {
                    loot.Add(new Item(new Info("sword", "a sword. a dagger, but longer."), ItemType.Mellee, ui, map, new Point(-1, -1)));
                }
            }
        }
        else if (loottype == 5)
        {
            for (var i = 0; i < rng.Next(1, 2); i++)
            {
                // 20% chance of a copy of 'gaseous tome', else its chance of a copy of 'how to do pyromancy' book.
                if (rng.Next(100) <= 20)
                {
                    loot.Add(new Item(new Info("gaseous tome", "a tome... that stinks a lot."), ItemType.Book, ui, map, new Point(-1, -1)));
                }
                else
                {
                    loot.Add(new Item(new Info("how to do pyromancy", "learn pyromancy! this is fine."), ItemType.Book, ui, map, new Point(-1, -1)));
                }
            }
        }


            // 50% chance of a chest to not contain loot.
            if (rng.Next(100) >= 50)
            {
                return loot;
            }
            else
            {
                return new List<Item>();
            }
        }
    }
