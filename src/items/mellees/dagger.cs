using MIST.AI;
using SadConsole;
using SadRogue.Primitives;

namespace MIST.items.Mellees
{
    public class dagger : IMelleeItem
    {
        public ItemType Type => ItemType.Mellee;

        public Info Info => new Info("dagger", "a dagger. really sharp.");

        private GameObject ThisObject;
        public GameObject? Object { get => ThisObject; set => ThisObject = value; }

        public int actioncost => 25;

        public dagger(UI ui, IScreenSurface surface)
        {
            ThisObject = new GameObject(new ColoredGlyph(Color.Orange, Color.Black, '/'), new Point(0, 0), surface, null, Info, null, ui);
        }

        public int CritWeaponAttack(GameObject target)
        {
            return 7;
        }

        public int WeaponAttack(GameObject target)
        {
            return 5;
        }
    }
}