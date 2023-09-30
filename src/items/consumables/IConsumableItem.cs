using MIST.AI;
using SadConsole;
using SadRogue.Primitives;

namespace MIST.items.consumables
{
    /// <summary>
    ///  base interface for all consumables
    /// </summary>
    public interface  IConsumableItem : IItem
    {
        /// <summary>
        /// trigers the use of this consumable
        /// </summary>
        void Use(UI ui, GameObject player);
    }
}