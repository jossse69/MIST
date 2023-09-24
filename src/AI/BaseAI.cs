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


        public void AITurn(GameObject go, Map map, List<GameObject> objects, GameObject player) 
        {
            var rng = new Random();
            var WANDER_RANGE = 6;

            // if tis dead, do nothing
            if (go == null || go.Fighter == null || go.Fighter.IsDead)
            {
                return;
            }

            // player alive, and monster is in fov
            if (!player.Fighter.IsDead && map.IsInFov(go.Position.X, go.Position.Y))
            {
                Target = player.Position;
            } else if (Target == new Point(-1, -1))
            {
                
                while (true) {
                    var x = go.Position.X + rng.Next(-WANDER_RANGE, WANDER_RANGE);
                    var y = go.Position.Y + rng.Next(-WANDER_RANGE, WANDER_RANGE);

                    // check if the target is in the map
                    if (x >= 0 && x < map.Width && y >= 0 && y < map.Height)
                    {
                        // if its not in a wall, move to it
                        if (!go.IsBlocked(x, y, map, objects))
                        {
                            Target = new Point(x, y);
                            break;
                        }
                    }
                }
            }

            // move to target if its set
            if (Target != new Point(-1, -1))
            {
                    var x = 0;
                    var y = 0;
                    if (Target.X > go.Position.X)
                    {
                        x = 1;
                    } else if (Target.X < go.Position.X)
                    {
                        x = -1;
                    }
                    
                    if (Target.Y > go.Position.Y)
                    {
                        y = 1;
                    }
                    else if (Target.Y < go.Position.Y)
                    {
                        y = -1;
                    }

                    // if we cant move to (x,y), them set target to (-1, -1)
                    if (go.IsBlocked(go.Position.X + x, go.Position.Y + y, map, objects))
                    {
                        Target = new Point(-1, -1);
                    }

                    // got to the target, set it to (-1, -1)
                    if (go.Position == Target)
                    {
                        Target = new Point(-1, -1);
                    }else
                    {
                        go.MoveAndAttack(x,y, map, objects);
                    }
                
            }
       
        }

    }
}