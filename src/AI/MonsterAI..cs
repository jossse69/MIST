using GoRogue;
using GoRogue.Pathing;
using SadConsole;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MIST.AI
{
    public class IMonsterAI
    {
        Point Target { get; set; }

        int Energy { get; set; }

        public bool Canspendenergy { get; set; }

        public virtual void AITurn(GameObject go, Map map, List<GameObject> objects, GameObject player)
        {
            throw new NotImplementedException();
        }

        public virtual void UpdateEnergy()
        {
            throw new NotImplementedException();
        }
    }

}