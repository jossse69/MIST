using MIST.AI;
using SadConsole;
using SadRogue.Primitives;

namespace MIST.items
{
    public interface IItem
    {
        ColoredGlyph itemAppearance {get=>this.itemAppearance; set{}}
        ItemType Type { get; }
        Info Info { get; }
        GameObject? Object { get; set; }
    }
}