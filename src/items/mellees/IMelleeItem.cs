using MIST.AI;
using SadConsole;
using SadRogue.Primitives;

namespace MIST.items.Mellees
{
    /// <summary>
    /// Base interface for melee items.
    /// </summary>
    public interface  IMelleeItem : IItem
    {
        int actioncost { get; }

        /// <summary>
        /// returns the DMG value of this mellee
        /// </summary>
        /// <param name="target"> the target of the attack, useful for status effects</param>
        /// <returns>the DMG value</returns>
        public abstract int WeaponAttack(GameObject target);

        /// <summary>
        /// returns the crit DMG value of this mellee
        /// </summary>
        /// <param name="target"> the target of the attack, useful for status effects</param>
        /// <returns>the crit DMG value</returns>
        public abstract int CritWeaponAttack(GameObject target);
    }

}
