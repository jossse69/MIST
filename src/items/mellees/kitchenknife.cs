using MIST.AI;
using SadConsole;
using SadRogue.Primitives;

namespace MIST.items.Mellees
{
    public class kitchenknife : IMelleeItem
    {
        public ItemType Type => ItemType.Mellee;

        public Info Info => new Info("kitchen knife", "a kitchen knife. usable.");

        private GameObject ThisObject;
        public GameObject? Object { get => ThisObject; set => ThisObject = value; }

        public int actioncost => 100;

        public kitchenknife(UI ui, IScreenSurface surface)
        {
            ThisObject = new GameObject(new ColoredGlyph(Color.Orange, Color.Black, '/'), new Point(0, 0), surface, null, Info, null, ui);
        }

        public int CritWeaponAttack(GameObject target)
        {
            return 5;
        }

        public int WeaponAttack(GameObject target)
        {
            return 4;
        }
    }
}