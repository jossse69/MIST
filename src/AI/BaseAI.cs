using GoRogue;
using GoRogue.Pathing;
using SadConsole;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;

namespace MIST.AI
{
    internal class MonsterAI
    {
        public Point Target { get; private set; } = new Point(-1, -1);


        public MonsterAI(Map map) 
        {

            
        }

        public void AITurn(GameObject go, Map map, List<GameObject> objects, GameObject player) 
        {
            // if tis dead, do nothing
            if (go == null || go.Fighter == null || go.Fighter.IsDead)
            {
                return;
            }

            // player alive, and monster is in fov
            if (!player.Fighter.IsDead && map.IsInFov(go.Position.X, go.Position.Y))
            {
                Target = player.Position;
            }

            // move to target if its set
            if (Target != new Point(-1, -1))
            {
                if (Target.X > go.Position.X)
                {
                    go.MoveAndAttack(1, 0, map, objects);
                } else if (Target.X < go.Position.X)
                {
                    go.MoveAndAttack(-1, 0, map, objects);
                }
                
                if (Target.Y > go.Position.Y)
                {
                    go.MoveAndAttack(0, 1, map, objects);
                }
                else if (Target.Y < go.Position.Y)
                {
                    go.MoveAndAttack(0, -1, map, objects);
                }
            }
            

       
        }

    }
}